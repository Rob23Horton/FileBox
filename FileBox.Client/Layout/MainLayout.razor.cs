namespace FileBox.Client.Layout;

public partial class MainLayout
{
	private bool SidebarOpen = false;

	protected override async Task OnInitializedAsync()
	{
		await folderService.StartAsync();
		await fileService.GetFilesFromAPI();

		folderService.CurrentFolderChanged += PageChanged;
		fileService.CurrentFileChanged += PageChanged;

		await base.OnInitializedAsync();
	}

	private void ToggleSidebar() => SidebarOpen = !SidebarOpen;

	private void PageChanged(object o)
	{
		SidebarOpen = false;
	}
}
