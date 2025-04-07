using FileBoxApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxApp.Services
{
	public interface IRecentFilesRepository
	{
		public Task<List<RecentFile>> GetRecentFiles();
		public Task ClearRecentFiles();
		public Task AddRecentFile(RecentFile RecentFile);
	}
}
