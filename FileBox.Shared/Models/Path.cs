using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblPath")]
	public class Path
	{
		[NameCast("PathId")]
		[PropertyType("INTEGER", true)]
		public int Id { get; set; }

		[PropertyType("INTEGER")]
		public int FileCode { get; set; }

		[PropertyType("VARCHAR(48)")]
		public string DeviceName { get; set; }

		[PropertyType("TEXT")]
		public string FilePath { get; set; }

		[PropertyType("INTEGER")]
		public int SortOrder { get; set; }
	}
}
