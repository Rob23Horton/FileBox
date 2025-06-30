using FileBox.Shared.Models;

namespace FileBoxWebApp.Client.Services
{
	public interface IUploadService
	{
		public void AddUploadFile(FileBoxFile File, byte[] Data);
		public List<UploadStatus> GetFilesUploading();
		public void DeleteUploadFile(int Id);

		public void ChangeId(int Id, int NewId);
		public void ChangeValue(int Id, string ValueName, bool NewValue);
		public void IncrementDataIndex(int Id);

		public void AddAction(string Id, Action Action);
		public void RemoveAction(string Id);
	}
}
