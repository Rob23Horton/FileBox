using DatabaseConnector.Attributes;

namespace FileBox.Shared.Models
{
	[Table("tblFolder")]
	public class ServerFolder
	{
		[NameCast("FolderId")]
		public int Id { get; set; }
		public string FilePath { get; set; }
		public bool CurrentlyActive { get; set; }
		public int? ParentCode { get; set; }
		public string? Prefix { get; set; }

		public ServerFolder Clone()
		{
			return new ServerFolder
			{
				Id = Id,
				FilePath = FilePath,
				CurrentlyActive = CurrentlyActive,
				Prefix = Prefix,
				ParentCode = ParentCode
			};
		}
	}
}
