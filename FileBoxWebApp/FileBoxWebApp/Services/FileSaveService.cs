using DatabaseConnector.Services;
using DatabaseConnector.Models;
using FileBox.Shared.Models;
using FileBoxWebApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace FileBoxWebApp.Services
{
	public class FileSaveService : IFileSaveService
	{
		private List<FileSave> FileSaves = new List<FileSave>();
		private IDatabaseConnector _connection;
		public FileSaveService(IDatabaseConnector connection)
		{
			_connection = connection;
		}

		public int StartFileSave(FileBoxFile File)
		{
			FileSave newFile = new FileSave()
			{
				Id = FileSaves.Count() + 1 == int.MaxValue ? 0 : FileSaves.Count() + 1,
				File = File.Clone()
			};

			FileSaves.Add(newFile);

			return newFile.Id;
		}

		public void AddDataToFile(UploadData FileData)
		{
			FileSave? file = FileSaves.FirstOrDefault(f => f.Id == FileData.Id);

			if (file is null)
			{
				return;
			}

			file.Data.Add(new KeyValuePair<int, List<byte>>(FileData.DataIndex, FileData.Data));
		}

		public void CancelFileSave(int Id)
		{
			FileSaves.RemoveAll(fs => fs.Id == Id);
		}

		public void SaveFile(int Id, int PathCode, int TotalFileSize)
		{
			//Gets current save
			FileSave? file = FileSaves.FirstOrDefault(f => f.Id == Id);

			if (file is null)
			{
				throw new Exception("Upload doesn't exist.");
			}

			//Checks that all the bytes are sent from the client
			int fileByteSize = file.Data.Select(d => d.Value).Sum(bytes => bytes.Count());
			if (TotalFileSize != fileByteSize)
			{
				throw new Exception("Not all data has been transfered.");
			}

			//Add file to database
			_connection.Insert<FileBoxFile>(file.File.Clone());

			//Get Id
			Select fileSelect = new Select("tblFile");
			fileSelect.AddWhere("Name", file.File.Name);
			fileSelect.AddWhere("Created", file.File.Created);
			fileSelect.AddWhere("Type", file.File.Type);
			FileBoxFile? addedFile = _connection.Select<FileBoxFile>(fileSelect).FirstOrDefault();

			if (addedFile == default)
			{
				throw new Exception("File wasn't added.");
			}
			file.File = addedFile.Clone();

			//Checks path exists
			Select pathSelect = new Select();
			fileSelect.AddWhere("PathId", PathCode);
			ServerPath? path = _connection.Select<ServerPath>(pathSelect).FirstOrDefault();

			if (path == default)
			{
				throw new Exception("Path doesn't exist.");
			}

			//Save File at path with the name of ID
			WriteFileToStorage(file, path);


			//Adds FileCode & Path to tblFilePath
			FilePath filePath = new FilePath() { FileCode = (int)addedFile.Id, PathCode = PathCode };
			_connection.Insert<FilePath>(filePath);
		}

		private void WriteFileToStorage(FileSave FileSave, ServerPath FilePath)
		{
			string filePath = Path.Combine(FilePath.FilePath, $"{FileSave.File.Id}.{FileSave.File.Type}");

			//Gets all bytes and puts them into one list
			List<byte> byteList = new List<byte>();
			foreach (KeyValuePair<int, List<byte>> currentData in FileSave.Data.OrderBy(d => d.Key))
			{
				byteList.AddRange(currentData.Value.ToArray());
			}

			//Saves file to storage
			File.WriteAllBytes(filePath, byteList.ToArray());

			Console.WriteLine($"File {FileSave.File.Id} (Name - {FileSave.File.Name}) written to path {FilePath.FilePath}");

		}
	}
}
