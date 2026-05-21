using Dapper;
using FileBox.Models.DBObjects;
using FileBox.Services;
using FileBox.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace FileBox.Controllers
{
	[ApiController]
	[Route("api/file")]
	public class FileController(IDbConnection DB, IHubContext<NotificationHub> Hub) : ControllerBase
	{

		[HttpGet]
		public async Task<List<Shared.Models.File>> GetFile()
		{
			try
			{
				List<DBFile> files = (await DB.QueryAsync<DBFile>("SELECT FileId, Type, Name, ParentFolderCode, Extension FROM tblFile")).ToList();

				//TODO - Check that user can see file (Permission on parent folder)

				return files.Select(f => new Shared.Models.File() { Id = f.FileId, Name = f.Name, FolderCode = f.ParentFolderCode, Extension = f.Extension, Type = f.Type }).ToList();
			}
			catch (Exception ex)
			{
				//TODO - Log Error
				return [];
			}
		}


		[HttpGet("create")]
		public async Task<IActionResult> CreateFile(Shared.Models.File File)
		{

			//Notify clients to get folders
			await Hub.Clients.All.SendAsync("FileStructureChanged");

			//TODO - Create folder

			return Ok(new ApiResponse<bool>() { Success = false, ErrorMessage = "Implimentation not created" });
		}

	}
}
