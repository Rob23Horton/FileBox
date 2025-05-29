using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Interfaces
{
	public interface ITagRepository
	{
		public Tag GetTagById(int Id);
		public List<Tag> GetAllTags();
		public List<Tag> GetLikeTags(string Name);
		public void AddTag(Tag tag);
		public void EditTag(Tag tag);
		public void DeleteTag(int Id);
		public int NoOfFiles(int Id);
	}
}
