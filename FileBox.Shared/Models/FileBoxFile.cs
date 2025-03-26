using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFile")]
	public class FileBoxFile
	{
		[NameCast("FileId")]
		public Int64 Id { get; set; }
		public string Name { get; set; }
		public DateTime Created { get; set; }
		public string Type { get; set; }
	}
}
