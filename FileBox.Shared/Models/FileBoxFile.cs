using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFile")]
	public class FileBoxFile
	{
		public FileBoxFile Clone()
		{
			return new FileBoxFile() {  Id = Id, Name = Name, Created = Created, Type = Type };
		}

		[NameCast("FileId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }

		[PropertyType("TEXT")]
		public string Name { get; set; }

		public DateTime Created { get; set; }

		[PropertyType("VARCHAR(12)")]
		public string Type { get; set; }
	}
}
