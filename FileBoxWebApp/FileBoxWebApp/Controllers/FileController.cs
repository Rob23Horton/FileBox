using FileBoxWebApp.Models;
using FileBoxWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FileBoxWebApp.Controllers
{
	[ApiController]
	[Route("api/File")]
	public class FileController : ControllerBase
	{
		private readonly FileService _fileService;
		public FileController(FileService fileService)
		{
			_fileService = fileService;
		}


		[HttpGet("Get/{FileId}")]
		public IActionResult GetFile(int FileId)
		{
			try
			{
				return Ok(_fileService.GetFile(FileId));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.ToString());
			}
		}

	}
}
