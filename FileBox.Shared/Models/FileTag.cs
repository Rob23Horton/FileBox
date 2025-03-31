using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFileTag")]
	public class FileTag
	{
		[NameCast("FileTagId")]
		[PropertyType("INTEGER", true)]
		public Int64 Id { get; set; }

		[PropertyType("INTEGER")]
		public Int64 FileCode { get; set; }

		[PropertyType("INTEGER")]
		public Int64 TagCode {  get; set; }

		public bool CurrentlyActive { get; set; }
	}
}
