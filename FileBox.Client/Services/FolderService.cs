using FileBox.Client.Components.FolderNavigation;
using FileBox.Shared.Models;
using FileBox.Shared.Models.Search;
using FileBox.Shared.Models.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace FileBox.Client.Services;

public class FolderService : IDisposable
{
	public FolderService(NavigationManager NavigationManager, HttpClient HttpClient)
	{
		_navigationManager = NavigationManager;
		_httpClient = HttpClient;

		_hub = new HubConnectionBuilder()
			.WithUrl($"{_navigationManager.BaseUri}hubs/notifications")
			.WithAutomaticReconnect()
			.Build();

		_hub.On("FolderStructureChanged", async () =>
		{
			await GetFoldersFromAPI();
		});
	}

	#region Private Properties

	private NavigationManager _navigationManager;
	private HubConnection _hub;
	private HttpClient _httpClient;

	private List<Folder> _folders = new List<Folder>();

	#endregion


	#region Events

	public event Action? FolderStructureChanged;
	public event Action<Folder>? CurrentFolderChanged;

	#endregion


	#region Public Methods

	public async Task StartAsync()
	{
		await _hub.StartAsync();
	}

	public async void SelectFolder(int? FolderId)
	{
		//For resetting folder selected (Go on root)
		if (FolderId is null)
			CurrentFolderChanged?.Invoke(new Folder() { Id = 0, Name = "" }); //Use ID of 0 as there will never be a folder with that ID

		//Loads folders from API if no folder exists
		if (!_folders.Any(f => f.Id == FolderId))
		{
			await GetFoldersFromAPI();
		}

		if (_folders.FirstOrDefault(f => f.Id == FolderId) is Folder folder)
			CurrentFolderChanged?.Invoke(folder);
	}

	public List<Folder> GetRootFolders() => _folders.Where(f => f.ParentFolderCode is null && !f.IsPrivate).ToList();

	public List<Folder> GetChildFolders(int FolderId) => _folders.Where(f => f.ParentFolderCode == FolderId && !f.IsPrivate).ToList();

	public async Task<Folder?> GetFolder(int FolderId)
	{
		//Loads folders from API if no folder exists
		if (!_folders.Any(f => f.Id == FolderId))
		{
			await GetFoldersFromAPI();
		}

		return _folders.FirstOrDefault(f => f.Id == FolderId);
	}

	public List<Models.FolderItem> GetFolderItems(int FolderId)
	{
		return _folders
			.Where(f => f.ParentFolderCode == FolderId)
			.Select(f => new Models.FolderItem()
			{
				Id = f.Id,
				Name = f.Name,
				ParentFolderCode = (int)f.ParentFolderCode!,
				Type = Shared.Models.Navigation.ItemType.Folder
			}).ToList();
	}

	public async Task<List<Models.FolderItem>> GetParentFolders(int FolderId)
	{
		if ((await GetFolder(FolderId)) is Folder folder && folder is { ParentFolderCode: not null })
		{
			if ((await GetFolder((int)folder.ParentFolderCode)) is Folder parentFolder)
				return [.. (await GetParentFolders(parentFolder.Id)), new Models.FolderItem() { Id = parentFolder.Id, Name = parentFolder.Name, ParentFolderCode = parentFolder.ParentFolderCode, Type = Shared.Models.Navigation.ItemType.Folder }];
		}
		return [];
	}


	public async Task GetFoldersFromAPI()
	{
		try
		{
			//Call API
			_folders = await _httpClient.GetFromJsonAsync<List<Folder>>("http://localhost:5072/api/folder") ?? [];

			//Tell components to update
			if (FolderStructureChanged is not null)
				FolderStructureChanged?.Invoke();
		}
		catch
		{

		}
	}

	public async Task<List<SearchFolder>> GetSearchFolders()
	{
		try
		{
			return await _httpClient.GetFromJsonAsync<List<SearchFolder>>("http://localhost:5072/api/folder/search") ?? [];
		}
		catch
		{

		}
		return [];
	}

	public List<Folder> GetAllFolders()
	{
		try
		{
			GetFoldersFromAPI();

			return _folders;
		}
		catch
		{

		}
		return [];
	}

	#endregion

	public void Dispose()
	{
		_hub.DisposeAsync();
	}
}
