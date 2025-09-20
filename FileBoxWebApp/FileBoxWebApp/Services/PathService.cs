using DatabaseConnector.Models;
using DatabaseConnector.Services;
using FileBox.Shared.Models;

namespace FileBoxWebApp.Services
{
	public class PathService
	{
		private readonly IDatabaseConnector _databaseConnector;
		public PathService(IDatabaseConnector databaseConnector)
		{
			_databaseConnector = databaseConnector;
		}


		public string GetPathFromFolderId(int FolderId)
		{
			Select ParentSelect = new Select();
			ParentSelect.AddWhere("FolderId", FolderId);
			ServerFolder? parentFolder = _databaseConnector.Select<ServerFolder>(ParentSelect).FirstOrDefault();

			if (parentFolder is null)
				throw new Exception("Folder doesn't exist.");

			List<ServerFolder> pathFolders = new List<ServerFolder>() { parentFolder.Clone() };

			if (parentFolder.ParentCode != null)
			{

				List<ServerFolder> folders = _databaseConnector.Select<ServerFolder>(new Select());

				GetParentFolder((int)parentFolder.ParentCode, folders, ref pathFolders);

			}

			pathFolders.Reverse();

			string path = "";

			foreach (ServerFolder folder in pathFolders)
			{
				path = $"{path}{folder.Prefix}{folder.FilePath}/";
			}

			return path;
		}

		private void GetParentFolder(int FolderId, List<ServerFolder> Folders, ref List<ServerFolder> PathFolders)
		{

			ServerFolder? parentFolder = Folders.FirstOrDefault(x => x.Id == FolderId);

			if (parentFolder is null)
				return;

			PathFolders.Add(parentFolder.Clone());

			if (parentFolder.ParentCode != null)
				GetParentFolder((int)parentFolder.ParentCode, Folders, ref PathFolders);
		}

	}
}
