using Dapper;
using FileBox.Models.DBObjects;
using FileBox.Services;
using FileBox.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;

namespace FileBox.Controllers
{
	[ApiController]
	[Route("api/tag")]
	public class TagControllers(IDbConnection DB, IHubContext<NotificationHub> Hub) : ControllerBase
	{
		[HttpGet]
		public async Task<List<Tag>> GetFolders()
		{
			try
			{
				List<DBTag> tags = (await DB.QueryAsync<DBTag>("SELECT TagId, Name, ParentTagCode, Colour FROM tblTag")).ToList();

				return tags.Select(f => new Tag() { Id = f.TagId, Name = f.Name, ParentTagCode = f.ParentTagCode, Colour = f.Colour }).ToList();
			}
			catch (Exception ex)
			{
				//TODO - Log Error
				return [];
			}
		}
	}
}
