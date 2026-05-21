using Microsoft.AspNetCore.Components;

namespace FileBox.Client.Pages;

public partial class File
{

	[Parameter, EditorRequired]
	public string FileId
	{
		get => fileId.ToString();
		set
		{
			if (int.Parse(value) is int intVal && intVal != fileId)
			{
				fileId = intVal;
				fileService.SelectFile(fileId);
			}
		}
	}
	private int fileId;

	private Shared.Models.File? file = null;

	protected override async Task OnInitializedAsync()
	{
		fileService.CurrentFileChanged += CurrentFileChanged;

		await base.OnInitializedAsync();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
			fileService.SelectFile(fileId);

		base.OnAfterRender(firstRender);
	}

	public void Dispose()
	{
		fileService.CurrentFileChanged -= CurrentFileChanged;
	}

	private void CurrentFileChanged(Shared.Models.File File)
	{
		try
		{
			fileId = File.Id;
			file = File;

			folderService.SelectFolder(file.FolderCode);

			StateHasChanged();
		}
		catch { }
	}

}
