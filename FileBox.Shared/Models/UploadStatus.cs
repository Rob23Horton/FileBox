using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models
{
	public class UploadStatus
	{
		public int Id { get; set; }
		public int ServerId { get; set; }

		public bool IsCancelled { get; set; }
		public bool IsPaused { get; set; }
		public bool IsWaiting { get; set; }

		public int Percentage { get; set; }

		public int TotalDataLength { get; set; }
		public int CurrentDataIndex { get; set; } = 0;

		public FileBoxFile File { get; set; }
		public int FolderCode { get; set; }
	}
}
