using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblBaseFileInfo")]
	public class BaseFileInfo
	{
		[NameCast("BaseFileInfoId")]
		[PropertyType("INTEGER", true)]
		public Int64 Id { get; set; }

		[PropertyType("INTEGER")]
		public Int64 FileCode { get; set; }

		[PropertyType("INTEGER")]
		public Int64 Size { get; set; }
		public DateTime LastModified { get; set; }
	}
}
