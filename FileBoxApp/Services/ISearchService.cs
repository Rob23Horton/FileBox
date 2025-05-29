using FileBoxApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxApp.Services
{
	public interface ISearchService
	{
		public Task<List<SearchFolder>> GetAllSearchFolders();
		public Task UpdateSearchFolders(List<SearchFolder> Folders);
		public Task<List<FileInfo>> SearchFolders();
	}
}
