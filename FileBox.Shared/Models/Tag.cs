using DatabaseConnector.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models
{
	[Table("tblTag")]
	public class Tag
	{
		[NameCast("TagId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }
		[PropertyType("VARCHAR(48)")]
		public string Name { get; set; }
	}
}
