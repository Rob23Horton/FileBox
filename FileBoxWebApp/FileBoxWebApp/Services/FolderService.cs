using DatabaseConnector.Models;
using DatabaseConnector.Services;
using FileBox.Shared.Models;
using FileBoxWebApp.Models;

namespace FileBoxWebApp.Services
{
	public class FolderService
	{
		private readonly IDatabaseConnector _databaseConnector;
		private readonly PathService _pathService;
		public FolderService(IDatabaseConnector databaseConnector, PathService pathService)
		{
			_databaseConnector = databaseConnector;
			_pathService = pathService;
		}

		public ServerFolder GetFolder(int FolderId)
		{
			Select FolderSelect = new Select();
			FolderSelect.AddWhere("FolderId", FolderId);
			FolderSelect.AddWhere("CurrentlyActive", 1);
			return _databaseConnector.Select<ServerFolder>(FolderSelect).First();
		}

		public ServerFolderContent GetFolderContent(int FolderId)
		{
			ServerFolderContent content = new ServerFolderContent();

			content.FolderId = FolderId;

			ServerFolder folder = GetFolder(FolderId);

			if (folder.ParentCode != null)
			{
				content.ParentFolder = GetFolder((int)folder.ParentCode);
			}

			Select folderSelect = new Select();
			folderSelect.AddWhere("ParentCode", FolderId);
			folderSelect.AddWhere("CurrentlyActive", 1);
			List<ServerFolder> childrenFolders = _databaseConnector.Select<ServerFolder>(folderSelect);

			Select fileSelect = new Select();
			fileSelect.AddWhere("tblFilePath", "PathCode", FolderId);
			List<FileWithPath> childrenFiles = _databaseConnector.Select<FileWithPath>(fileSelect);

			childrenFolders.ForEach(f => content.Items.Add(new FolderContent() { Id = f.Id, Name = f.FilePath, Type = "folder" }));
			childrenFiles.ForEach(f => content.Items.Add(new FolderContent() { Id = (int)f.Id, Name = f.Name, Type = f.Type }));

			return content;
		}
	}
}
