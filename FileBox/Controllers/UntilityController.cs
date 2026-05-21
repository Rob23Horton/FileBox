using FileBox.Shared.Models.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using FileBox.Models.Navigation;

namespace FileBox.Controllers
{
	[ApiController]
	[Route("api/utilities")]
	public class UntilityController(IDbConnection DB) : Controller
	{

		private readonly string _iconPath = "Resources/Icons/FolderItem/";

		[HttpGet("icon/{ItemType}")]
		public async Task<IActionResult> GetFolderItemIcon(ItemType ItemType)
		{
			try
			{
				Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";

				string? path = null;
				int counter = 0;

				ItemType currItemType = ItemType;

				while (path is null)
				{
					counter++;

					if (counter > 5)
						throw new Exception($"Image Icon couldn't be loaded, ItemType - {ItemType}");

					DBFolderItem? item = await DB.QueryFirstAsync<DBFolderItem?>(
						"SELECT FolderItemId, ImageName, FallbackFolderItemCode FROM tblFolderItem WHERE FolderItemId = @Id",
						new
						{
							Id = currItemType
						});

					if (item is null)
						return NotFound();

					path = Path.GetFullPath(Path.Combine(_iconPath, item.ImageName));

					if (!System.IO.File.Exists(path))
					{
						if (item.FallbackFolderItemCode is null)
							return NotFound();

						path = null;
						currItemType = (ItemType)item.FallbackFolderItemCode;
					}
				}
				if (path is null)
					throw new Exception($"Image Icon couldn't be loaded, ItemType - {ItemType}");

				//Sets it is cached if the counter isn't higher than 1 go
				if (counter < 2)
					Response.Headers.CacheControl = "public, max-age=86400";


				return PhysicalFile(path, "image/png");
			}
			catch (Exception ex)
			{
				//TODO - Log Error

				return BadRequest();
			}
		}

	}
}
