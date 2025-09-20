using FileBox.Shared.Models;
using Microsoft.JSInterop;
using System;
using System.IO;

namespace FileBoxWebApp.Client.Services
{
	public class UploadService : IUploadService
	{
		private readonly IJSRuntime _JSRuntime;
		public UploadService(IJSRuntime JSRuntime)
		{
			_JSRuntime = JSRuntime;
		}

		private List<UploadStatus> Files { get; set; } = new List<UploadStatus>();
		private Dictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();

		public async void AddUploadFile(FileBoxFile File, int FolderCode, byte[] Data, string Type)
		{
			//Creates the id for the new upload from either the next increment or 0
			int id = 0;
			UploadStatus? file = Files.OrderBy(f => f.Id).LastOrDefault();
			if (file != null && file.Id < int.MaxValue)
			{
				id = file.Id + 1;
			}

			UploadStatus newUpload = new UploadStatus()
			{
				Id = id,
				Percentage = 0,
				IsWaiting = true,
				TotalDataLength = Data.Length,
				File = File.Clone(),
				FolderCode = FolderCode
			};

			Files.Add(newUpload);

			//Adds data to IndexedDB
			await _JSRuntime.InvokeVoidAsync("binaryDb.putFile", "FileDb", "files", $"{newUpload.Id}", Data, Type);

			//Updates 
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

		public void SetServerId(int Id, int NewId)
		{
			UploadStatus? file = Files.Where(f => f.Id == Id).FirstOrDefault();

			if (file is null)
			{
				return;
			}

			file.ServerId = NewId;
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
