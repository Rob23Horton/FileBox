using FileBox.Shared.Models;
using DatabaseConnector.Attributes;

namespace FileBoxWebApp.Models
{
	[Table("tblFile")]
	public class FileWithPath : FileBoxFile
	{
		[Join("tblFile", "FileId", "tblFilePath", "FileCode")]
		[SourceTable("tblFilePath")]
		public int PathCode { get; set; }
	}
}
