using DatabaseConnector.Attributes;

namespace FileBoxWebApp.Models
{
	[Table("tblPath")]
	public class ServerPath
	{
		[NameCast("PathId")]
		public int Id { get; set; }
		public string FilePath { get; set; }
		public bool CurrentlyActive { get; set; }
	}
}
