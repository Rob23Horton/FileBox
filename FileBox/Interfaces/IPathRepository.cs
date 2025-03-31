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
		Public Path GetPathById(int Id);
		Public List<Path> GetAllPathsFromFileCode(int FileCode);
		Public void ChangeCurrentAciveFilePath(int FileCode, int PathId);
		Public void AddPath(Path Path);
		Public void EditPath(Path Path);
		Public void RemovePath(int Id);
	}
}