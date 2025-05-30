﻿using DatabaseConnector.Models;
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
	public class TagRepository : ITagRepository
	{
		private IDatabaseConnector _databaseConnector;
		public TagRepository(IDatabaseConnector _databaseConnector)
		{
			this._databaseConnector = _databaseConnector;
		}

		public void AddTag(Tag tag)
		{
			_databaseConnector.Insert<Tag>(tag);
		}

		public void DeleteTag(int Id)
		{
			_databaseConnector.Delete(new Tag { Id = Id });

			//Deletes all the files that have this tag
			Select TagSelect = new Select();
			TagSelect.AddWhere("TagCode", Id);

			List<FileTag> FilesWithTag = _databaseConnector.Select<FileTag>(TagSelect);
			foreach (FileTag fileTag in FilesWithTag)
			{
				_databaseConnector.Delete(fileTag);
			}
		}

		public void EditTag(Tag tag)
		{
			_databaseConnector.Update<Tag>(tag, new Update());
		}

		public List<Tag> GetAllTags()
		{
			List<Tag> Tags = _databaseConnector.Select<Tag>(new Select());

			return Tags;
		}

		public List<Tag> GetLikeTags(string Name)
		{
			Select TagSelect = new Select();
			TagSelect.AddWhere("", "Name", Name, true);

			List<Tag> Tags = _databaseConnector.Select<Tag>(TagSelect);

			return Tags;
		}

		public Tag GetTagById(int Id)
		{
			Select TagSelect = new Select();
			TagSelect.AddWhere("TagId", Id);

			List<Tag> Tags = _databaseConnector.Select<Tag>(TagSelect);

			if (Tags.Count() == 0)
			{
				return new Tag();
			}

			return Tags[0];
		}

		public int NoOfFiles(int Id)
		{
			Select TagSelect = new Select();
			TagSelect.AddWhere("TagCode", Id);

			List<FileTag> FilesWithTag = _databaseConnector.Select<FileTag>(TagSelect);

			return FilesWithTag.Count();
		}
	}
}
