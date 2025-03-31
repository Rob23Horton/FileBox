using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblBaseFileInfo")]
	public class BaseFileInfo
	{
		[NameCast("BaseFileInfoId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }

		[PropertyType("INTEGER")]
		public long FileCode { get; set; }

		[PropertyType("INTEGER")]
		public long Size { get; set; }
		public DateTime LastModified { get; set; }
	}
}
