using FileBox.Shared.Models;

namespace FileBoxWebApp.Services
{
	public interface IFileSaveService
	{
		public int StartFileSave(FileBoxFile File);
		public void AddDataToFile(UploadData FileData);
		public void CancelFileSave(int Id);
		public void SaveFile(int Id, int PathCode, int TotalFileSize);

	}
}
