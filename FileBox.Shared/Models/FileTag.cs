using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFileTag")]
	public class FileTag
	{
		[NameCast("FileTagId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }

		[PropertyType("INTEGER")]
		public long FileCode { get; set; }

		[PropertyType("INTEGER")]
		public long TagCode {  get; set; }

		public bool CurrentlyActive { get; set; }
	}
}
