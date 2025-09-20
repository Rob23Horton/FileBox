using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models
{
	public class UploadStart
	{
		public string Name { get; set; }
		public DateTime Created { get; set; }
		public string Type { get; set; }
	}
}
