using FileBox.Shared.Models.Navigation;

namespace FileBox.Models.DBObjects;

public class DBFile
{
	public int FileId { get; set; }
	public ItemType Type { get; set; }
	public string Name { get; set; }
	public int ParentFolderCode { get; set; }
	public required string Extension { get; set; }
}
