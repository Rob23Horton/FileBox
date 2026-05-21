using FileBox.Shared.Models.Search;
using Microsoft.AspNetCore.Components;

namespace FileBox.Client.Components.Admin;

public partial class SearchFolders
{

	public List<SearchFolder> _folders = new List<SearchFolder>();


	protected async override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			_folders = await folderService.GetSearchFolders();
			StateHasChanged();
		}

		await base.OnAfterRenderAsync(firstRender);
	}

}
