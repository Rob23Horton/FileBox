using FileBox.Shared.Models;

namespace FileBoxWebApp.Models
{
	public class FileSave
	{
		public int Id { get; set; }
		public FileBoxFile File { get; set; }
		public List<KeyValuePair<int, List<byte>>> Data { get; set; } = new List<KeyValuePair<int, List<byte>>>();
	}
}
