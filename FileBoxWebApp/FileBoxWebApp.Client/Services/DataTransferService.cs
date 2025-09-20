using FileBox.Shared.Models;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FileBoxWebApp.Client.Services
{
	public class DataTransferService : IDataTransferService
	{
		private readonly IJSRuntime _JSRuntime;
		private readonly HttpClient _httpClient;
		private readonly IUploadService _uploadService;
		private bool _isRunning = false;
		private int _maxFilesCount = 4;
		private int _byteDataSize = 200_000;
		private int _waitTime = 500;
		private List<KeyValuePair<int, List<byte>>> CurrentData = new List<KeyValuePair<int, List<byte>>>();
		public DataTransferService(IJSRuntime JSRuntime, HttpClient httpClient, IUploadService uploadService)
		{
			_JSRuntime = JSRuntime;
			_httpClient = httpClient;
			_uploadService = uploadService;

			UpdateWaitTime();
		}

		public async void StartService()
		{
			if (_isRunning)
			{
				return;
			}

			_isRunning = true;
			_uploadService.AddAction(this.ToString() is not null ? this.ToString() : "DataTransferService", UpdateWaitTime);

			CurrentData = new List<KeyValuePair<int, List<byte>>>();

			while (_isRunning)
			{
				await Task.Delay(_waitTime);

				//Gets all files
				List<UploadStatus> files = _uploadService.GetFilesUploading();
				if (files.Count() == 0) continue;

				//Cancelling
				foreach (UploadStatus cancelledFile in files.Where(f => f.IsCancelled))
				{
					CancelFileUpload(cancelledFile);
				}

				//Getting current files
				List<UploadStatus> currentFiles = files.Where(f => !f.IsWaiting && !f.IsCancelled && f.Percentage < 100).ToList();

				//Filling up currentFiles
				if (currentFiles.Count() < _maxFilesCount)
				{
					//Gets the new addingFiles
					int filesWaitingLength = files.Count(f => f.IsWaiting);
					List<UploadStatus> addingFiles = files.Where(f => f.IsWaiting).ToList().GetRange(0, filesWaitingLength < _maxFilesCount - currentFiles.Count() ? filesWaitingLength : _maxFilesCount - currentFiles.Count());

					foreach (UploadStatus file in addingFiles)
					{
						bool success = await StartFileUpload(file);
						if (success)
						{
							currentFiles.Add(file);
						}
					}
				}

				//Upload data
				foreach (UploadStatus file in currentFiles.Where(cf => !cf.IsPaused))
				{
					if (!CurrentData.Any(d => d.Key == file.Id))
					{
						continue;
					}

					await UploadStage(file);

					if (CurrentData.Single(d => d.Key == file.Id).Value.Count() <= 0)
					{
						await FinishFileUpload(file);
					}
				}

			}

		}

		private async Task<bool> StartFileUpload(UploadStatus File)
		{
			//Tells server that new 
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync<UploadStart>($"api/Upload/Start", new UploadStart() { Name = File.File.Name, Created = File.File.Created, Type = File.File.Type });
			int? fileId = await response.Content.ReadFromJsonAsync<int>();

			if (fileId is null)
			{
				return false;
			}

			//Gets data and adds it to current data
			try
			{
				byte[] Data = await _JSRuntime.InvokeAsync<byte[]>("binaryDb.getFileAsBytes", "FileDb", "files", $"{File.Id}");
				CurrentData.Add(new KeyValuePair<int, List<byte>>(File.Id, Data.ToList()));
			}
			catch (Exception ex) //If data isn't there then cancel starting upload
			{
				Console.WriteLine(ex);
				return false;
			}

			_uploadService.ChangeValue(File.Id, "Waiting", false);
			_uploadService.SetServerId(File.Id, (int)fileId);
			return true;
		}

		private async Task UploadStage(UploadStatus File)
		{
			List<byte> Data = CurrentData.Single(d => d.Key == File.Id).Value;

			//Gets next data to send
			List<byte> nextDataRange = Data.Count > _byteDataSize ? Data.GetRange(0, _byteDataSize) : Data;

			//Uploads data to server
			HttpResponseMessage success = await _httpClient.PostAsJsonAsync<UploadData>($"api/Upload/Upload", new UploadData() { Id = File.ServerId, DataIndex = File.CurrentDataIndex, Data = nextDataRange });

			if (success is null || !success.IsSuccessStatusCode)
			{
				return;
			}

			//Updates percentage
			float percentage = ((float)Data.Count() / (float)File.TotalDataLength) * 100f;
			File.Percentage = (int)(100f - percentage);

			//Increments data
			_uploadService.IncrementDataIndex(File.Id);

			//Updates the data to remove amount
			Data.RemoveRange(0, nextDataRange.Count());
		}

		private async Task FinishFileUpload(UploadStatus File)
		{
			_uploadService.ChangeValue(File.Id, "Paused", true);

			//Sends message to server to save the file in the right place
			await _httpClient.PostAsJsonAsync<UploadFinish>($"api/Upload/Finish", new UploadFinish() { Id = File.ServerId, PathCode = File.FolderCode, TotalByteSize = File.TotalDataLength});

			//TODO - Check if all data hasn't been sent and add the missing parts to the data needed to be sent

			CurrentData.RemoveAll(d => d.Key == File.Id);
			await _JSRuntime.InvokeVoidAsync("binaryDb.deleteFile", "FileDb", "files", $"{File.Id}");

			File.Percentage = 100;
			_uploadService.ChangeValue(File.Id, "Paused", false);

			UpdateWaitTime();
		}

		private async void CancelFileUpload(UploadStatus File)
		{
			//Asks server to cancel upload
			HttpResponseMessage? response = null;
			try
			{
				response = await _httpClient.PostAsJsonAsync<int>($"api/Upload/Cancel", File.ServerId);
			}
			catch
			{
				
			}

			if (response is null || !response.IsSuccessStatusCode || await response.Content.ReadFromJsonAsync<bool>() == false)
			{
				return;
			}


			//Deletes CurrentData Item
			CurrentData.RemoveAll(d => d.Key == File.Id);

			//Deletes IndexedDB Item
			await _JSRuntime.InvokeVoidAsync("binaryDb.deleteFile", "FileDb", "files", $"{File.Id}");

			//Deletes UploadService Item
			_uploadService.DeleteUploadFile(File.Id);
		}

		public void UpdateWaitTime()
		{
			if (_uploadService.GetFilesUploading().Where(f => !f.IsCancelled && f.Percentage < 100).Count() == 0)
			{
				_waitTime = 1000;
			}
			else if (_uploadService.GetFilesUploading().Where(f => f.IsPaused).Count() >= _maxFilesCount)
			{
				_waitTime = 500;
			}
			else
			{
				_waitTime = 1;
			}
		}

		public void StopService()
		{
			_isRunning = false;
		}
	}
}
