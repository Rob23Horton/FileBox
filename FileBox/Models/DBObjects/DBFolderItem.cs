using FileBox.Shared.Models.Navigation;

namespace FileBox.Models.Navigation
{
	public class DBFolderItem
	{
		public ItemType FolderItemId { get; set; }
		public required string ImageName { get; set; }
		public ItemType? FallbackFolderItemCode { get; set; } = null;
	}
}
