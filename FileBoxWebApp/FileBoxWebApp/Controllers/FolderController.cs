using FileBoxWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileBoxWebApp.Controllers
{
	[ApiController]
	[Route("api/Folder")]
	public class FolderController : ControllerBase
	{
		
		private readonly FolderService _folderService;
		public FolderController(FolderService folderService)
		{
			_folderService = folderService;
		}

		[HttpGet("GetFolder/{FolderId}")]
		public IActionResult GetFolder(int FolderId)
		{
			try
			{
				return Ok(_folderService.GetFolder(FolderId));
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpGet("GetContent/{FolderId}")]
		public IActionResult GetFolderContent(int FolderId)
		{
			try
			{
				return Ok(_folderService.GetFolderContent(FolderId));
			}
			catch
			{
				return BadRequest();
			}
		}
	}
}
