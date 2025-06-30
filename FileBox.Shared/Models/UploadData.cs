using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models
{
	public class UploadData
	{
		public int Id { get; set; }
		public int DataIndex { get; set; }
		public List<byte> Data { get; set; }
	}
}
