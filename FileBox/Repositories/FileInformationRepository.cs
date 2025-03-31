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
	public class FileInformationRepository : IFileInformationRepository
	{
		private readonly IDatabaseConnector _databaseConnector;
		public FileInformationRepository(IDatabaseConnector _databaseConnector)
		{
			this._databaseConnector = _databaseConnector;
		}

		public InfoValue GetFileInformationById(int Id)
		{
			Select select = new Select();
			select.AddWhere("InfoValueId", Id);

			List<InfoValue> fileInfos = _databaseConnector.Select<InfoValue>(select);

			if (fileInfos.Count == 0)
			{
				return new InfoValue();
			}

			return fileInfos[0];
		}

		public InfoValue GetFileInformationByName(int FileCode, string Name)
		{
			Select select = new Select();
			select.AddWhere("FileCode", FileCode);
			select.AddWhere("ValueName", Name);

			List<InfoValue> fileInfos = _databaseConnector.Select<InfoValue>(select);

			if (fileInfos.Count == 0)
			{
				return new InfoValue();
			}

			return fileInfos[0];
		}

		public List<InfoValue> GetAllInfoFromFileCode(int FileCode)
		{
			Select select = new Select();
			select.AddWhere("FileCode", FileCode);

			return _databaseConnector.Select<InfoValue>(select);
		}

		public void AddFileInformation(InfoValue FileInfo)
		{
			_databaseConnector.Insert<InfoValue>(FileInfo);
		}

		public void EditFileInformation(InfoValue FileInfo)
		{
			_databaseConnector.Update<InfoValue>(FileInfo, new Update());
		}

		public void DeleteFileInformation(int Id)
		{
			_databaseConnector.Delete<InfoValue>(new InfoValue() { Id = Id });
		}

	}
}
