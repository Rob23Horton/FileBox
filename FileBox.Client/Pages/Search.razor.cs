using FileBox.Client.Components.FolderNavigation;
using FileBox.Client.Services;
using Microsoft.AspNetCore.Components;

namespace FileBox.Client.Pages;

public partial class Search
{

	[Parameter]
	public string SearchText { get; set; } = string.Empty;


	private SearchState _state = SearchState.Waiting;
	private List<Models.FolderItem> _results = new List<Models.FolderItem>();

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			folderService.FolderStructureChanged += StateHasChanged;
			folderService.FolderStructureChanged += GoSearch;
			folderService.SelectFolder(null);

			if (!string.IsNullOrWhiteSpace(SearchText))
				GoSearch();
		}

		base.OnAfterRender(firstRender);
	}

	public void Dispose()
	{
		folderService.FolderStructureChanged -= StateHasChanged;
		folderService.FolderStructureChanged -= GoSearch;
	}


	private void GoSearch()
	{
		try
		{
			if (string.IsNullOrWhiteSpace(SearchText))
			{
				_state = SearchState.Waiting;
				return;
			}

			_state = SearchState.Searching;
			_results = [];

			var folders = folderService.GetAllFolders().Where(f => f.Name.Contains(SearchText));
			var files = fileService.GetAllFiles().Where(f => f.Name.Contains(SearchText));

			_results = 
				[..folders.Select(f => new Models.FolderItem() { Id = f.Id, Name = f.Name, ParentFolderCode = f.ParentFolderCode, Type = Shared.Models.Navigation.ItemType.Folder}),
				..files.Select((f) => new Models.FolderItem() { Id = f.Id, Name = $"{f.Name}.{f.Extension}", ParentFolderCode = f.FolderCode, Type = f.Type })];

			_state = SearchState.Completed;
		}
		catch (Exception ex)
		{
			_state = SearchState.Failed;
		}
		StateHasChanged();
	}


	enum SearchState
	{
		Waiting,
		Searching,
		Completed,
		Failed
	}
	
}
