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
	public class FileRepository : IFileRepository
	{
		private IDatabaseConnector _databaseConnector;
		public FileRepository(IDatabaseConnector _databaseConnector)
		{
			this._databaseConnector = _databaseConnector;
		}

		public FileBoxFile GetFileById(int Id)
		{
			Select select = new Select();
			select.AddWhere("FileId", Id);

			List<FileBoxFile> files = _databaseConnector.Select<FileBoxFile>(select);

			if (files.Count() == 0)
			{
				return new FileBoxFile();
			}

			return files[0];
		}

		public List<FileBoxFile> GetAllFiles()
		{
			return _databaseConnector.Select<FileBoxFile>(new Select());
		}

		public List<FileBoxFile> GetFilesLikeName(string Name)
		{
			Select select = new Select();
			select.AddWhere("", "Name", Name, true);

			return _databaseConnector.Select<FileBoxFile>(select);
		}

		public void AddFile(FileBoxFile File)
		{
			_databaseConnector.Insert<FileBoxFile>(File);
		}

		public void EditFile(FileBoxFile File)
		{
			_databaseConnector.Update<FileBoxFile>(File, new Update());
		}

		public void DeleteFile(int Id)
		{
			_databaseConnector.Delete<FileBoxFile>(new FileBoxFile() {Id = Id});
		}

		public List<FileTag> GetTagsForFile(int FileId)
		{
			Select select = new Select();
			select.AddWhere("FileCode", FileId);

			return _databaseConnector.Select<FileTag>(select);
		}

		public void AddTagForFile(int FileId, int TagCode)
		{
			FileTag newTag = new FileTag();
			newTag.FileCode = FileId;
			newTag.TagCode = TagCode;
			newTag.CurrentlyActive = true;

			_databaseConnector.Insert<FileTag>(newTag);
		}

		public void RemoveTagForFile(int FileId, int TagCode)
		{
			Select select = new Select();
			select.AddWhere("FileCode", FileId);
			select.AddWhere("TagCode", TagCode);

			List<FileTag> fileTags = _databaseConnector.Select<FileTag>(select);

			if (fileTags.Count() == 0)
			{
				throw new Exception($"File {FileId} doesn't have tag {TagCode}");
			}

			//Delete the tag
			_databaseConnector.Delete<FileTag>(fileTags[0]);
		}

		public int GetFileId(FileBoxFile File)
		{
			Select select = new Select();
			select.AddWhere("Name", File.Name);
			select.AddWhere("Type", File.Type);

			List<FileBoxFile> files = _databaseConnector.Select<FileBoxFile>(select);

			if (files.Count() == 0)
			{
				throw new Exception($"File wasn't found for {File.Name}.{File.Type}.");
			}

			return (int)files[0].Id;
		}
	}
}