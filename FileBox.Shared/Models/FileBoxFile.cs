using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFile")]
	public class FileBoxFile
	{
		[NameCast("FileId")]
		[PropertyType("INTEGER", true)]
		public Int64 Id { get; set; }

		[PropertyType("TEXT")]
		public string Name { get; set; }

		public DateTime Created { get; set; }

		[PropertyType("VARCHAR(12)")]
		public string Type { get; set; }
	}
}
