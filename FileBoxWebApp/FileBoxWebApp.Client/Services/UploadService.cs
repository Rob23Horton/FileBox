using FileBox.Shared.Models;

namespace FileBoxWebApp.Client.Services
{
	public class UploadService : IUploadService
	{
		private List<UploadStatus> Files { get; set; } = new List<UploadStatus>();
		private Dictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();

		public void AddUploadFile(FileBoxFile File, byte[] Data)
		{
			UploadStatus newUpload = new UploadStatus()
			{
				Id = Files.Count() == 0 ? 1 : Files.OrderBy(f => f.Id).Last().Id + 1,
				Percentage = 0,
				IsWaiting = true,
				Data = Data.ToList(),
				TotalDataLength = Data.Length,
				File = File.Clone(),
			};

			Files.Add(newUpload);

			InvokeActions();
		}

		public List<UploadStatus> GetFilesUploading()
		{
			try
			{
				return Files;
			}
			catch
			{
				return new List<UploadStatus>();
			}
		}

		public void ChangeId(int Id, int NewId)
		{
			UploadStatus? file = Files.Where(f => f.Id == Id).FirstOrDefault();

			if (file is null)
			{
				return;
			}

			file.Id = NewId;
		}
		public void ChangeValue(int Id, string ValueName, bool NewValue)
		{
			UploadStatus? file = Files.Where(f => f.Id == Id).FirstOrDefault();

			if (file is null)
			{
				return;
			}

			switch (ValueName.ToLower())
			{
				case "cancelled":
					file.IsCancelled = NewValue;
					break;

				case "paused":
					file.IsPaused = NewValue;
					break;

				case "waiting":
					file.IsWaiting = NewValue;
					break;

				default:
					return;
			}

			InvokeActions();
		}
		public void IncrementDataIndex(int Id)
		{
			UploadStatus? file = Files.Where(f => f.Id == Id).FirstOrDefault();

			if (file is null)
			{
				return;
			}

			file.CurrentDataIndex++;

			InvokeActions();
		}

		public void DeleteUploadFile(int Id)
		{
			UploadStatus? file = Files.Where(f => f.Id == Id).FirstOrDefault();

			if (file is null)
			{
				return;
			}

			Files.Remove(file);
		}

		public void AddAction(string Id, Action Action)
		{
			Actions.Add(Id, Action);
		}
		private void InvokeActions()
		{
			//Invokes action for all functions to update
			foreach (Action action in Actions.Values)
			{
				try
				{
					action.Invoke();
				}
				catch
				{

				}
			}
		}
		public void RemoveAction(string Id)
		{
			Actions.Remove(Id);
		}
	}
}
