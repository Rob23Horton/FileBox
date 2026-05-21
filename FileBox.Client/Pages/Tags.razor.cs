using FileBox.Client.Services;

namespace FileBox.Client.Pages;

public partial class Tags
{
	protected override Task OnInitializedAsync()
	{
		tagService.TagsUpdated += StateHasChanged;

		return base.OnInitializedAsync();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			folderService.SelectFolder(null);
		}

		base.OnAfterRender(firstRender);
	}

	public void Dispose()
	{
		tagService.TagsUpdated -= StateHasChanged;
	}
}
