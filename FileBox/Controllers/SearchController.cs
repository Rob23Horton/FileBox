using Dapper;
using FileBox.Models.DBObjects;
using FileBox.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FileBox.Controllers
{
	[ApiController]
	[Route("api/search")]
	public class SearchController(IDbConnection DB) : ControllerBase
	{

		[HttpGet("folder/{SearchText}")]
		public async Task<List<Folder>> GetFoldersLikeSearch(string SearchText)
		{
			try
			{
				List<DBFolder> folders = (await DB.QueryAsync<DBFolder>(
					"SELECT FolderId, Name, ParentFolderCode, RequiredPermission FROM tblFolder WHERE Name LIKE @Name",
					new { Name = $"'{SearchText}'"}
					)).ToList();

				//TODO - Get all folder permissions for user and filter folders

				return folders.Select(f => new Folder() { Id = f.FolderId, Name = f.Name, IsPrivate = false, ParentFolderCode = f.ParentFolderCode, Permission = f.RequiredPermission }).ToList();
			}
			catch (Exception ex)
			{
				//TODO - Log Error
			}
			return [];
		}

		[HttpGet("file/{SearchText}")]
		public async Task<List<Shared.Models.File>> GetFilesLikeSearch(string SearchText)
		{
			try
			{
				List<DBFile> files = (await DB.QueryAsync<DBFile>(
					"SELECT FileId, Type, Name, ParentFolderCode, Extension FROM tblFile WHERE Name LIKE @Name",
					new { Name = $"%{SearchText}%"
					})).ToList();

				//TODO - Check that user can see file (Permission on parent folder)

				return files.Select(f => new Shared.Models.File() { Id = f.FileId, Name = f.Name, FolderCode = f.ParentFolderCode, Extension = f.Extension, Type = f.Type }).ToList();
			}
			catch (Exception ex)
			{
				//TODO - Log Error
			}
			return [];
		}

	}
}
