using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFileTag")]
	public class FileTag
	{
		[NameCast("FileTagId")]
		[PropertyType("INTEGER", true)]
		public int Id { get; set; }

		[PropertyType("INTEGER")]
		public int FileCode { get; set; }

		[PropertyType("INTEGER")]
		public int TagCode {  get; set; }

		public bool CurrentlyActive { get; set; }

	}
}
