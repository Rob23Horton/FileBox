using Dapper;
using DocumentFormat.OpenXml.Packaging;
using FileBox.Models.DBObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using OpenXmlPowerTools;
using Sdcb.LibRaw;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Data;
using System.Xml.Linq;

namespace FileBox.Controllers
{
	[Route("api/filedata")]
	[ApiController]
	public class FileDataController(IDbConnection DB) : ControllerBase
	{

		#region Private Methods

		private async Task<DBFile?> GetFileFromDB(int FileId)
		{
			try
			{
				return await DB.QueryFirstOrDefaultAsync<DBFile>(
					"SELECT FileId, Type, Name, ParentFolderCode, Extension FROM tblFile WHERE FileId = @FileId",
					new
					{
						FileId = FileId
					});
			}
			catch
			{
				return null;
			}
		}


		private async Task<string?> GetFilePath(int FolderCode)
		{
			try
			{
				List<DBFolder> folders = (await DB.QueryAsync<DBFolder>("SELECT FolderId, Name, ParentFolderCode, FolderPath FROM tblFolder")).ToList();

				DBFolder folder;
				if (folders.FirstOrDefault(f => f.FolderId == FolderCode) is DBFolder parentFolder)
					folder = parentFolder;
				else
					return null;

				List<DBFolder> pathFolders = [parentFolder];

				if (parentFolder.ParentFolderCode != null)
					GetParentFolder((int)parentFolder.ParentFolderCode, folders, ref pathFolders);

				pathFolders.Reverse();

				string path = string.Join("/", pathFolders.Select(pf => pf.FolderPath).Where(pf => !string.IsNullOrWhiteSpace(pf)));

				return $"{path}/";
			}
			catch
			{
				return null;
			}
		}

		private void GetParentFolder(int FolderId, List<DBFolder> Folders, ref List<DBFolder> PathFolders)
		{

			DBFolder? parentFolder = Folders.FirstOrDefault(x => x.FolderId == FolderId);

			if (parentFolder is null)
				return;

			PathFolders.Add(parentFolder);

			if (parentFolder.ParentFolderCode != null)
				GetParentFolder((int)parentFolder.ParentFolderCode, Folders, ref PathFolders);
		}

		#endregion


		#region Public Methods

		[HttpGet("image/{FileId}")]
		public async Task<IActionResult> GetImageFile(int FileId)
		{
			try
			{
				//Get file from db
				DBFile file;
				if ((await GetFileFromDB(FileId)) is DBFile dbFile)
					file = dbFile;
				else
					return NotFound();

				if (file.Type != Shared.Models.Navigation.ItemType.Image)
					return NotFound();

				//TODO - Check user permission

				//Build Path
				string path;
				if ((await GetFilePath(file.ParentFolderCode) is string filePath))
					path = filePath;
				else
					return NotFound();

				//Checks if file exists
				if (!System.IO.File.Exists($"{path}{file.Name}.{file.Extension}"))
					return NotFound();

				//Gets the mimetype
				string mimeType;
				var provider = new FileExtensionContentTypeProvider();
				if (!provider.TryGetContentType($"{path}{file.Name}.{file.Extension}", out mimeType))
					return NotFound(); //Returned not found if file type failed

				//Check it is correct type (Image)
				if (!mimeType.ToUpper().StartsWith("IMAGE"))
					return NotFound();

				//Opens stream
				var stream = System.IO.File.OpenRead($"{path}{file.Name}.{file.Extension}");


				Response.Headers.CacheControl = "public, max-age=86400"; //Caches the result
				return new Microsoft.AspNetCore.Mvc.FileStreamResult(stream, mimeType);
			}
			catch
			{
				return NotFound();
			}
		}

		[HttpGet("rawimage/{FileId}")]
		public async Task<IActionResult> GetRawImage(int FileId)
		{
			try
			{
				//Get file from db
				DBFile file;
				if ((await GetFileFromDB(FileId)) is DBFile dbFile)
					file = dbFile;
				else
					return NotFound();

				if (file.Type != Shared.Models.Navigation.ItemType.RawImage)
					return NotFound();

				//Build Path
				string path;
				if ((await GetFilePath(file.ParentFolderCode) is string filePath))
					path = filePath;
				else
					return NotFound();

				//Checks if file exists
				if (!System.IO.File.Exists($"{path}{file.Name}.{file.Extension}"))
					return NotFound();


				//LibRaw stuff ---

				// 1) Load and process the RAW file with Sdcb.LibRaw
				using RawContext raw = RawContext.OpenFile($"{path}{file.Name}.{file.Extension}");

				// Unpack the RAW data and run the LibRaw demosaic pipeline
				raw.Unpack();
				raw.DcrawProcess();

				// Get a memory image from RawContext
				using ProcessedImage procImg = raw.MakeDcrawMemoryImage();

				int width = procImg.Width;
				int height = procImg.Height;

				// Get raw pixel data (8‑bit per channel)
				byte[] pixelData = procImg.AsSpan<byte>().ToArray();

				// 2) Load the pixel data into ImageSharp (RGB24 since processed is 3 channels)
				using Image<Rgb24> image = Image.LoadPixelData<Rgb24>(pixelData, width, height);

				// 3) Encode the image into a PNG stream
				MemoryStream outputStream = new MemoryStream();
				image.Save(outputStream, new PngEncoder());
				outputStream.Position = 0;


				//TODO - Caches file to local storage

				//Returns file to client (Making sure the client caches it)
				Response.Headers.CacheControl = "public, max-age=86400"; //Caches the result
				return new Microsoft.AspNetCore.Mvc.FileStreamResult(outputStream, "image/png");
			}
			catch (Exception ex)
			{
				return NotFound();
			}
		}


		[HttpGet("htmldocument/{FileId}")]
		public async Task<IActionResult> GetDocumentAsHtml(int FileId)
		{
			try
			{
				//Get file from db
				DBFile file;
				if ((await GetFileFromDB(FileId)) is DBFile dbFile)
					file = dbFile;
				else
					return NotFound();

				if (file.Type != Shared.Models.Navigation.ItemType.Document)
					return NotFound();

				//TODO - Check user permission

				//Build Path
				string path;
				if ((await GetFilePath(file.ParentFolderCode) is string filePath))
					path = filePath;
				else
					return NotFound();

				if (!Path.Exists($"{path}{file.Name}.{file.Extension}"))
					return NotFound();


				//Converts docx to html
				using var wordDoc = WordprocessingDocument.Open($"{path}{file.Name}.{file.Extension}", true);

				var settings = new HtmlConverterSettings
				{
					PageTitle = Path.GetFileNameWithoutExtension($"{path}{file.Name}.{file.Extension}"),
					FabricateCssClasses = true,
					CssClassPrefix = "docx-",

					ImageHandler = imageInfo =>
					{
						using var ms = new MemoryStream();

						// Encode the SKBitmap to PNG or JPEG
						SKEncodedImageFormat format = imageInfo.ContentType switch
						{
							"image/png" => SKEncodedImageFormat.Png,
							"image/jpeg" => SKEncodedImageFormat.Jpeg,
							"image/jpg" => SKEncodedImageFormat.Jpeg,
							"image/gif" => SKEncodedImageFormat.Png, // GIF → PNG fallback
							_ => SKEncodedImageFormat.Png
						};

						// Save SKBitmap to MemoryStream
						imageInfo.SKBitmap.Encode(ms, format, 100); // quality = 100

						string base64 = Convert.ToBase64String(ms.ToArray());

						return new XElement(Xhtml.img,
							new XAttribute("src",
								$"data:{imageInfo.ContentType};base64,{base64}"),
							imageInfo.ImgStyleAttribute
						);
					}
				};

				XElement html = HtmlConverter.ConvertToHtml(wordDoc, settings);

				return Content(html.ToString(SaveOptions.DisableFormatting), "text/html");
			}
			catch
			{
				return NotFound();
			}
		}

		#endregion
	}
}
