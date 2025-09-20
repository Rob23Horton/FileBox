using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models
{
	public class FolderContent
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Size { get; set; }
		public DateTime? Added { get; set; }
		public string Type { get; set; }
	}
}
