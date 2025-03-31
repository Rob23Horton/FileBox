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

		Public Path GetPathById(int Id)
		{
			Select select = new Select();
			select.AddWhere("PathId", Id);

			List<Path> paths = _databaseConnector.Select<Path>(select);

			if (paths.Count() == 0)
			{
				return new Path();
			}

			return paths[0];
		}

		Public List<Path> GetAllPathsFromFileCode(int FileCode)
		{
			Select select = new Select();
			select.AddWhere("FileCode", FileCode);

			return _databaseConnector.Select<Path>(select);
		}

		Public void ChangeCurrentAciveFilePath(int FileCode, int PathId)
		{
			Select update = new Select();
			update.AddWhere("FileCode", FileCode);

			Path plainPath = new Path();
			plainPath.CurrentlyActive = false;

			//Sets all paths to unactive
			_databaseConnector.Update<Path>(plainPath, update);


			update.AddWhere("PathId", PathId);
			
			Path activePath = new Path();
			activePath.CurrentlyActive = true;

			//Sets the correct path to active
			_databaseConnector.Update<Path>(activePath, update);
		}

		Public void AddPath(Path Path)
		{
			_databaseConnector.Insert<Path>(Path);
		}

		Public void EditPath(Path Path)
		{
			_databaseConnector.Update<Path>(Path, new Select());
		}

		Public void RemovePath(int Id)
		{
			_databaseConnector.Delete<Path>(new Path { Id = Id });
		}
	}
}