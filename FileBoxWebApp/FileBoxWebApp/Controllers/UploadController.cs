using FileBox.Shared.Models;
using FileBoxWebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileBoxWebApp.Controllers
{
	[ApiController]
	[Route("api/Upload")]
	public class UploadController : ControllerBase
	{
		private readonly IFileSaveService _saveService;
		public UploadController(IFileSaveService saveService)
		{
			this._saveService = saveService;
		}

		[HttpPost("Start")]
		[ProducesResponseType(200, Type=typeof(int))]
		[ProducesResponseType(400)]
		public IActionResult Start(UploadStart File)
		{
			try
			{
				return Ok(_saveService.StartFileSave(new FileBoxFile() { Name = File.Name, Created = File.Created, Type = File.Type }));
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
				return Ok();
			}
			catch
			{
				return BadRequest();
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
