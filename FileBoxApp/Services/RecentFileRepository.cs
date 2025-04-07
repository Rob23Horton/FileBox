using Blazored.LocalStorage;
using FileBoxApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxApp.Services
{
	public class RecentFileRepository : IRecentFilesRepository
	{
		private readonly ILocalStorageService _localStorage;
		public RecentFileRepository(ILocalStorageService localStorage)
		{
			_localStorage = localStorage;
		}

		public async Task AddRecentFile(RecentFile RecentFile)
		{
			List<RecentFile> RecentFiles = await this.GetRecentFiles();

			//Removes the oldest one when the list is 25 long
			if (RecentFiles.Count() > 25)
			{
				RecentFile FileToRemove = RecentFiles.OrderBy(r => r.Accessed).First();

				RecentFiles.Remove(FileToRemove);
			}

			//Adds the new file and saves it back to the local storage
			RecentFiles.Add(RecentFile);
			await _localStorage.SetItemAsync("RecentFiles", RecentFiles);
		}

		public async Task ClearRecentFiles()
		{
			await _localStorage.SetItemAsync("RecentFiles", new List<RecentFile>());
		}

		public async Task<List<RecentFile>> GetRecentFiles()
		{
			try
			{
				List<RecentFile> RecentFiles = await _localStorage.GetItemAsync<List<RecentFile>>("RecentFiles");
				return RecentFiles is null ? new List<RecentFile>() : RecentFiles;
			}
			catch
			{
				return new List<RecentFile>();
			}
		}
	}
}
