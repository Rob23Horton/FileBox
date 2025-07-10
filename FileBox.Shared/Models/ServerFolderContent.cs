using FileBox.Shared.Models;

namespace FileBox.Shared.Models
{
	public class ServerFolderContent
	{
		public int FolderId { get; set; }
		public ServerFolder? ParentFolder { get; set; }
		public List<FolderContent> Items { get; set; } = new List<FolderContent>();
	}
}
