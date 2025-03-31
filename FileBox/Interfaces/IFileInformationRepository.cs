using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Interfaces
{
	public interface IFileInformationRepository
	{
		public InfoValue GetFileInformationById(int Id);
		public InfoValue GetFileInformationByName(int FileCode, string Name);
		public List<InfoValue> GetAllInfoFromFileCode(int FileCode);
		public void AddFileInformation(InfoValue FileInfo);
		public void EditFileInformation(InfoValue FileInfo);
		public void DeleteFileInformation(int Id);
	}
}
