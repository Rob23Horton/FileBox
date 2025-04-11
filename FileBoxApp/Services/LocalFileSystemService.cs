using FileBox.Shared.Models;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace FileBoxApp.Services
{
	public class LocalFileSystemService : ILocalFileSystemService
	{
		public async Task<byte[]> GetFile(FileBoxFile FileBoxFile, FileBoxPath Path)
		{
			try
			{
				string filePath = $"{Path.FilePath}{FileBoxFile.Name}.{FileBoxFile.Type}";
				return await File.ReadAllBytesAsync(filePath);
			}
			catch
			{
				return new byte[0];
			}
		}

		public async Task<string> GetTextFile(FileBoxFile FileBoxFile, FileBoxPath Path)
		{
			try
			{
				string filePath = $"{Path.FilePath}{FileBoxFile.Name}.{FileBoxFile.Type}";
				return await File.ReadAllTextAsync(filePath);
			}
			catch
			{
				return String.Empty;
			}
		}

		public async Task<byte[]> GetJPGFile(FileBoxFile File, FileBoxPath Path)
		{
			try
			{
				byte[] imageBytes = await GetFile(File, Path);

				using (var inputStream = new MemoryStream(imageBytes))
				using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(inputStream)) // Auto-detect format
				using (var outputStream = new MemoryStream())
				{
					image.Save(outputStream, new JpegEncoder()); // Save as JPEG
					return outputStream.ToArray();
				}
			}
			catch
			{
				return new byte[0];
			}
		}
	}
}
