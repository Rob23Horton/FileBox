using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Interfaces
{
	public interface IPathRepository
	{
		public FileBoxPath GetPathById(int Id);
		public List<FileBoxPath> GetAllPathsFromFileCode(int FileCode);
		public void ChangeCurrentActiveFilePath(int FileCode, int PathId);
		public void AddPath(FileBoxPath Path);
		public void EditPath(FileBoxPath Path);
		public void RemovePath(int Id);
	}
}