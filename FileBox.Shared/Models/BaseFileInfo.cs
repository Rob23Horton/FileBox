using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblBaseFileInfo")]
	public class BaseFileInfo
	{
		[NameCast("BaseFileInfoId")]
		[PropertyType("INTEGER", true)]
		public int Id { get; set; }

		[PropertyType("INTEGER")]
		public int FileCode { get; set; }

		[PropertyType("INTEGER")]
		public int Size { get; set; }
		public DateTime LastModified { get; set; }
	}
}
