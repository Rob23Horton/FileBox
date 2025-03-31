using DatabaseConnector.Models;
using DatabaseConnector.Services;
using FileBox.Interfaces;
using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Repositories
{
	public class PathRepository : IPathRepository
	{
		private IDatabaseConnector _databaseConnector;
		public PathRepository(IDatabaseConnector _databaseConnector)
		{
			this._databaseConnector = _databaseConnector;
		}

		public FileBoxPath GetPathById(int Id)
		{
			Select select = new Select();
			select.AddWhere("PathId", Id);

			List<FileBoxPath> paths = _databaseConnector.Select<FileBoxPath>(select);

			if (paths.Count() == 0)
			{
				return new FileBoxPath();
			}

			return paths[0];
		}

		public List<FileBoxPath> GetAllPathsFromFileCode(int FileCode)
		{
			Select select = new Select();
			select.AddWhere("FileCode", FileCode);

			return _databaseConnector.Select<FileBoxPath>(select);
		}

		public void ChangeCurrentActiveFilePath(int FileCode, int PathId)
		{
			Update update = new Update();
			update.AddWhere("FileCode", FileCode);

			FileBoxPath plainPath = new FileBoxPath();
			plainPath.FileCode = FileCode;
			plainPath.CurrentlyActive = false;

			//Sets all paths to unactive
			_databaseConnector.Update<FileBoxPath>(plainPath, update);


			update.AddWhere("PathId", PathId);
			
			FileBoxPath activePath = new FileBoxPath();
			activePath.FileCode = FileCode;
			activePath.CurrentlyActive = true;

			//Sets the correct path to active
			_databaseConnector.Update<FileBoxPath>(activePath, update);
		}

		public void AddPath(FileBoxPath FileBoxPath)
		{
			_databaseConnector.Insert<FileBoxPath>(FileBoxPath);
		}

		public void EditPath(FileBoxPath Path)
		{
			_databaseConnector.Update<FileBoxPath>(Path, new Update());
		}

		public void RemovePath(int Id)
		{
			_databaseConnector.Delete<FileBoxPath>(new FileBoxPath { Id = Id });
		}
	}
}