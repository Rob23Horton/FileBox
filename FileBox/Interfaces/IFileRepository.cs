using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Interfaces
{
	public interface IFileRepository
	{
		public FileBoxFile GetFileById(int Id);
		public List<FileBoxFile> GetAllFiles();
		public List<FileBoxFile> GetFilesLikeName(string Name);

		public void AddFile(FileBoxFile File);
		public void EditFile(FileBoxFile File);
		public void DeleteFile(int Id);

		public List<FileTag> GetTagsForFile(int FileId);
		public void AddTagForFile(int FileId, int TagCode);
		public void RemoveTagForFile(int FileId, int TagCode);
	}
}