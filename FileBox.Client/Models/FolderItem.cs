using FileBox.Shared.Models.Navigation;

namespace FileBox.Client.Models
{
	public struct FolderItem
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public ItemType Type { get; set; }
		public int? ParentFolderCode { get; set; }
	}
}
