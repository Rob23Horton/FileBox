using FileBox.Client.Components.FolderNavigation;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;

namespace FileBox.Client.Services;

public class FileService(HttpClient _httpClient)
{
	#region Private Members

	private List<Shared.Models.File> _loadedFiles = new List<Shared.Models.File>();

	#endregion


	#region Public Events

	public event Action? FileStructureChanged;
	public event Action<Shared.Models.File>? CurrentFileChanged;

	#endregion

	#region Public Methods

	public List<Models.FolderItem> GetFolderItems(int FolderId)
	{
		return _loadedFiles
			.Where(f => f.FolderCode == FolderId)
			.Select((f) => new Models.FolderItem()
			{
				Id = f.Id,
				Name = $"{f.Name}.{f.Extension}",
				ParentFolderCode = f.FolderCode,
				Type = f.Type
			}).ToList();
	}

	public async void SelectFile(int FileId)
	{
		//If file is already loaded and skips loading from API
		if (GetFileFromLoaded(FileId) is Shared.Models.File file)
		{
			CurrentFileChanged?.Invoke(file);
			return;
		}

		//Load file from API
		await GetFilesFromAPI();

		//Sets file if it was loaded from API
		if (GetFileFromLoaded(FileId) is Shared.Models.File file2)
			CurrentFileChanged?.Invoke(file2);
	}

	public async Task<Shared.Models.File?> GetFile(int FileId)
	{
		//If file is already loaded
		if (GetFileFromLoaded(FileId) is Shared.Models.File file)
			return file;

		//Load file from API
		await GetFilesFromAPI();

		//Returns null or the loaded file again
		return GetFileFromLoaded(FileId);
	}

	private Shared.Models.File? GetFileFromLoaded(int FileId) => _loadedFiles.FirstOrDefault(f => f.Id == FileId);


	public async Task GetFilesFromAPI()
	{
		try
		{
			//Call API
			_loadedFiles = await _httpClient.GetFromJsonAsync<List<Shared.Models.File>>("http://localhost:5072/api/file") ?? [];

			//Tell components to update
			FileStructureChanged?.Invoke();
		}
		catch { }
	}

	public List<Shared.Models.File> GetAllFiles()
	{
		try
		{
			GetFilesFromAPI();

			return _loadedFiles;
		}
		catch
		{

		}
		return [];
	}

	#endregion
}
