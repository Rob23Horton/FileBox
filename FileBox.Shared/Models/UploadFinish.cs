using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models
{
	public class UploadFinish
	{
		public int Id { get; set; }
		public int PathCode { get; set; }
		public int TotalByteSize { get; set; }
	}
}
