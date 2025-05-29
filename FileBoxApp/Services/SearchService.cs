using Blazored.LocalStorage;
using FileBoxApp.Models;

namespace FileBoxApp.Services
{
	public class SearchService : ISearchService
	{
		private readonly ILocalStorageService _localStorageService;
		public SearchService(ILocalStorageService localStorageService)
		{
			_localStorageService = localStorageService;
		}

		public async Task<List<SearchFolder>> GetAllSearchFolders()
		{
			try
			{
				List<SearchFolder> folders = await _localStorageService.GetItemAsync<List<SearchFolder>>("SearchFolders");

				if (folders is null)
				{
					return new List<SearchFolder>();
				}

				return folders;
			}
			catch
			{
				return new List<SearchFolder>();
			}
		}

		public async Task<List<FileInfo>> SearchFolders()
		{
			try
			{
				List<FileInfo> files = new List<FileInfo>();

				List<SearchFolder> folders = await GetAllSearchFolders();

				foreach (SearchFolder folder in folders)
				{
					//Skips if it doesn't exist
					if (!Directory.Exists(folder.Path))
					{
						continue;
					}

					string[] folderFiles = Directory.GetFiles(folder.Path, "", SearchOption.AllDirectories);

					foreach (string file in folderFiles)
					{
						files.Add(new FileInfo(file));
					}
				}

				return files;
			}
			catch (Exception e)
			{
				return new List<FileInfo>();
			}
		}

		public async Task UpdateSearchFolders(List<SearchFolder> Folders)
		{
			await _localStorageService.SetItemAsync("SearchFolders", Folders);
		}
	}
}
