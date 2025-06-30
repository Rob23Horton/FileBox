using FileBox.Shared.Models;
using FileBoxWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileBoxWebApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UploadController : ControllerBase
	{
		private readonly IFileSaveService _saveService;
		public UploadController(IFileSaveService saveService)
		{
			_saveService = saveService;
		}

		[HttpPost("Start")]
		[ProducesResponseType(200, Type = typeof(int))]
		[ProducesResponseType(400)]
		public IActionResult Start([FromBody] FileBoxFile File)
		{
			try
			{
				return Ok(_saveService.StartFileSave(File));
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpPost("Upload")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult UploadData([FromBody] UploadData Data)
		{
			try
			{
				_saveService.AddDataToFile(Data);
				return Ok(true);
			}
			catch
			{
				return BadRequest(false);
			}
		}

		[HttpPost("Finish")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public IActionResult FinishUpload([FromBody]UploadFinish File)
		{
			try
			{
				_saveService.SaveFile(File.Id, File.PathCode, File.TotalByteSize);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}

		[HttpPost("Cancel")]
		public IActionResult Cancel([FromBody]int Id)
		{
			try
			{
				_saveService.CancelFileSave(Id);
				return Ok(true);

			}
			catch
			{
				return BadRequest(false);
			}
		}
	}
}
