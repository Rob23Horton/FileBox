using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxApp.Models
{
	public class RecentFile
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime Accessed { get; set; }
		public List<Tag> Tags { get; set; }
	}
}
