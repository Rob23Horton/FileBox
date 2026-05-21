using Dapper;
using FileBox.Models.DBObjects;
using FileBox.Models.Navigation;
using FileBox.Services;
using FileBox.Shared.Models;
using FileBox.Shared.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace FileBox.Controllers
{
	[ApiController]
	[Route("api/folder")]
	public class FolderController(IDbConnection DB, IHubContext<NotificationHub> Hub) : Controller
	{
		[HttpGet]
		public async Task<List<Shared.Models.Folder>> GetFolders()
		{
			try
			{
				List<DBFolder> folders = (await DB.QueryAsync<DBFolder>("SELECT FolderId, Name, ParentFolderCode, RequiredPermission FROM tblFolder")).ToList();

				//TODO - Get all folder permissions for user and filter folders

				return folders.Select(f => new Folder() { Id = f.FolderId, Name = f.Name, IsPrivate = false, ParentFolderCode = f.ParentFolderCode, Permission = f.RequiredPermission}).ToList();
			}
			catch (Exception ex)
			{
				//TODO - Log Error
				return [];
			}
		}


		[HttpGet("create")]
		public async Task<IActionResult> CreateFolder(Folder Folder)
		{

			//Notify clients to get folders
			await Hub.Clients.All.SendAsync("FolderStructureChanged");

			//TODO - Create folder

			return Ok(new ApiResponse<bool>() { Success = false, ErrorMessage = "Implimentation not created" });
		}


		[HttpGet("search")]
		public async Task<List<SearchFolder>> GetSearchFolders()
		{
			try
			{
				List<SearchFolder> searchFolders = (await DB.QueryAsync<SearchFolder>(@"
							SELECT
								f.FolderId as Id,
								f.Name, f.ParentFolderCode,
								f.RequiredPermission,
								s.ExcludeFiles As ExcludeFilesString,
								s.ExcludeFolders As ExcludeFoldersString
							FROM tblSearchFolder as s
							INNER JOIN tblFolder as f ON f.FolderId = s.FolderCode")).ToList();

				//TODO - Check permissions for user

				return searchFolders;
			}
			catch
			{
				//TODO - Log Error
				return [];
			}
		}
	}
}
