using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models;

public class FileExtended : File
{
	public DateTime UploadedDate { get; set; }
}
