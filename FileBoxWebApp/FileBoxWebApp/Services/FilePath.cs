using DatabaseConnector.Attributes;

namespace FileBoxWebApp.Services
{
	[Table("tblFilePath")]
	public class FilePath
	{
		[NameCast("FilePathId")]
		public int Id { get; set; }
		public int FileCode { get; set; }
		public int PathCode { get; set; }
	}
}
