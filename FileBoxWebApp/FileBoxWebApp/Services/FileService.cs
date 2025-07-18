using DatabaseConnector.Models;
using DatabaseConnector.Services;
using FileBox.Shared.Models;
using FileBoxWebApp.Models;

namespace FileBoxWebApp.Services
{
	public class FileService
	{
		private readonly PathService _pathService;
		private readonly IDatabaseConnector _databaseConnector;
		public FileService(PathService pathService, IDatabaseConnector databaseConnector)
		{
			_pathService = pathService;
			_databaseConnector = databaseConnector;
		}


		public FileLoad GetFile(int FileId)
		{
			//Gets file from db
			Select FileSelect = new Select();
			FileSelect.AddWhere("FileId", FileId);
			FileBoxFile file = _databaseConnector.Select<FileBoxFile>(FileSelect).Single();

			//Gets folder code from db
			Select filePathSelect = new Select();
			filePathSelect.AddWhere("FileCode", file.Id);
			FilePath filePath = _databaseConnector.Select<FilePath>(filePathSelect).Single();

			//Builder path from folder id
			string path = _pathService.GetPathFromFolderId(filePath.PathCode);

			//Gets data from path
			string sFilePath = Path.Combine(path, $"{file.Id}.{file.Type}");
			byte[] fileBytes = System.IO.File.ReadAllBytes(sFilePath);
			string base64 = Convert.ToBase64String(fileBytes);

			return new FileLoad() { Id = (int)file.Id, Name = file.Name, Type = file.Type, Data = base64 };
		}

	}
}
