using FileBox.Shared.Models.Security;

namespace FileBox.Models.DBObjects;

public class DBFolder
{
	public int FolderId { get; set; }
	public string Name { get; set; }
	public int? ParentFolderCode { get; set; } = null;
	public PermissionType RequiredPermission { get; set; } = PermissionType.Inherited;
	public string FolderPath { get; set; }

}
