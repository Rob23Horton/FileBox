using Microsoft.AspNetCore.Components;

namespace FileBox.Client.Pages;

public partial class Folder
{
	[Parameter, EditorRequired]
	public string FolderId
	{
		get => folderId.ToString();
		set
		{
			if (int.Parse(value) is int intVal && intVal != folderId)
			{
				folderId = intVal;
				folderService.SelectFolder(intVal);
			}
		}
	}
	private int folderId;

	private Shared.Models.Folder? folder = null;
	private bool popupFolder = false;

	protected override Task OnInitializedAsync()
	{
		folderService.CurrentFolderChanged += CurrentFolderChanged;
		folderService.FolderStructureChanged += StateHasChanged;

		return base.OnInitializedAsync();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
			folderService.SelectFolder(folderId);

		base.OnAfterRender(firstRender);
	}


	private void CurrentFolderChanged(Shared.Models.Folder Folder)
	{
		try
		{
			popupFolder = false;
			folderId = Folder.Id;
			folder = Folder;
			StateHasChanged();
		}
		catch { }
	}

	private void PopupFolderClicked()
	{
		popupFolder = !popupFolder;
	}

	public void Dispose()
	{
		folderService.CurrentFolderChanged -= CurrentFolderChanged;
		folderService.FolderStructureChanged -= StateHasChanged;
	}

}
