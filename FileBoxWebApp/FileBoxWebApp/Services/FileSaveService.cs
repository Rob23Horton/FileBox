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
		private List<FileSave> FileSaves { get; set; }
		private readonly IDatabaseConnector _connection;
		private readonly PathService _pathService;
		private int _byteDataSize = 200_000;
		public FileSaveService(IDatabaseConnector connection, PathService pathService)
		{
			FileSaves = new List<FileSave>();

			_connection = connection;
			_pathService = pathService;
		}

		public int StartFileSave(FileBoxFile File)
		{
			int id = 0;

			FileSave? lastSave = FileSaves.OrderBy(f => f.Id).LastOrDefault();
			if (lastSave != null && lastSave.Id < int.MaxValue)
			{
				id = lastSave.Id + 1;
			}
			else
			{
				id = 0;
			}

			FileSave newFile = new FileSave()
			{
				Id = id,
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

		public void SaveFile(int Id, int FolderCode, int TotalFileSize)
		{
			//Gets current save
			FileSave? file = FileSaves.FirstOrDefault(f => f.Id == Id);

			if (file is null)
			{
				throw new Exception("Upload doesn't exist.");
			}

			//Checks that all the bytes are sent from the client
			int fileByteSize = file.Data.Select(d => d.Value).Sum(bytes => bytes.Count());
			if (TotalFileSize > fileByteSize)
			{
				ThrowNotAllDataUploadedException(file, TotalFileSize);
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
			string path = _pathService.GetPathFromFolderId(FolderCode);

			//Save File at path with the name of ID
			WriteFileToStorage(file, path);


			//Adds FileCode & Path to tblFilePath
			FilePath filePath = new FilePath() { FileCode = (int)addedFile.Id, PathCode = FolderCode };
			_connection.Insert<FilePath>(filePath);

			FileSaves.RemoveAll(fs => fs.Id == file.Id);
		}

		private void ThrowNotAllDataUploadedException(FileSave FileSave, int TotalFileSize)
		{
			int totalPackets = TotalFileSize / _byteDataSize;

			List<int> missingPackets = new List<int>();

			//Increments through numbers to check what packets are missing
			int currentIndex = 0;
			while (currentIndex < totalPackets)
			{
				if (!FileSave.Data.Any(d => d.Key == currentIndex))
				{
					missingPackets.Add(currentIndex);
				}

				currentIndex++;
			}

			throw new NotAllDataUploadedException(missingPackets);
		}

		private void WriteFileToStorage(FileSave FileSave, string FilePath)
		{
			string filePath = Path.Combine(FilePath, $"{FileSave.File.Id}.{FileSave.File.Type}");

			//Gets all bytes and puts them into one list
			List<byte> byteList = new List<byte>();
			foreach (KeyValuePair<int, List<byte>> currentData in FileSave.Data.OrderBy(d => d.Key))
			{
				byteList.AddRange(currentData.Value.ToArray());
			}

			//Saves file to storage
			File.WriteAllBytes(filePath, byteList.ToArray());

			Console.WriteLine($"File {FileSave.File.Id} (Name - {FileSave.File.Name}) written to path {FilePath}");

		}
	}
}
