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
		public Tag Clone()
		{
			return new Tag() { Id = this.Id, Name = this.Name };
		}

		[NameCast("TagId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }
		[PropertyType("VARCHAR(48)")]
		public string Name { get; set; }
	}
}
