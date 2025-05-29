using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblPath")]
	public class FileBoxPath
	{
		public FileBoxPath Clone()
		{
			return new FileBoxPath() { Id = Id, FileCode = FileCode, DeviceName = DeviceName, FilePath = FilePath, CurrentlyActive = CurrentlyActive };
		}

		[NameCast("PathId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }

		[PropertyType("INTEGER")]
		public long FileCode { get; set; }

		[PropertyType("VARCHAR(48)")]
		public string DeviceName { get; set; }

		[PropertyType("TEXT")]
		public string FilePath { get; set; }

		public bool CurrentlyActive { get; set; }
	}
}
