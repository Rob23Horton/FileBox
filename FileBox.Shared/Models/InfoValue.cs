using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblInfoValue")]
	public class InfoValue
	{
		[NameCast("InfoValueId")]
		[PropertyType("INTEGER", true)]
		public long Id { get; set; }

		[PropertyType("INTEGER")]
		public long FileCode { get; set; }

		[PropertyType("VARCHAR(24)")]
		public string ValueName { get; set; }

		[PropertyType("TEXT")]
		public string Value { get; set; }
	}
}
