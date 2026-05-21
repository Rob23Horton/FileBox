using FileBox.Client.Models;
using FileBox.Shared.Models;

namespace FileBox.Client.Pages;

public partial class Home
{
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			folderService.FolderStructureChanged += StateHasChanged;
			folderService.SelectFolder(null);
		}

		base.OnAfterRender(firstRender);
	}

	public void Dispose()
	{
		folderService.FolderStructureChanged -= StateHasChanged;
	}

	private FolderItem ConvertFolderToItem(FileBox.Shared.Models.Folder Folder) =>
		new FolderItem()
		{
			Id = Folder.Id,
			Name = Folder.Name,
			ParentFolderCode = Folder.ParentFolderCode,
			Type = Shared.Models.Navigation.ItemType.Folder
		};

}
