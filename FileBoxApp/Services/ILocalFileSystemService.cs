using FileBox.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBoxApp.Services
{
	public interface ILocalFileSystemService
	{
		public Task<byte[]> GetFile(FileBoxFile File, FileBoxPath Path);
		public Task<string> GetTextFile(FileBoxFile File, FileBoxPath Path);
		public Task<byte[]> GetJPGFile(FileBoxFile File, FileBoxPath Path);
	}
}
