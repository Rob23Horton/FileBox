using FileBox.Shared.Models;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FileBoxWebApp.Client.Services
{
	public class DataTransferService : IDataTransferService
	{
		private readonly HttpClient _httpClient;
		private readonly IUploadService _uploadService;
		private bool _isRunning = false;
		private int _maxFilesCount = 4;
		private int _byteDataSize = 100_000;
		private int _waitTime = 500;
		public DataTransferService(HttpClient httpClient, IUploadService uploadService)
		{
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
						await StartFileUpload(file);
					}

					currentFiles.AddRange(addingFiles);
				}

				//Upload data
				foreach (UploadStatus file in currentFiles.Where(cf => !cf.IsPaused))
				{
					await UploadStage(file);

					if (file.Data.Count() == 0)
					{
						FinishFileUpload(file);
					}
				}

			}

		}

		private async Task<bool> StartFileUpload(UploadStatus File)
		{
			//Tells server that new 
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync<FileBoxFile>($"api/Upload/Start", File.File);

			if (response is null || !response.IsSuccessStatusCode)
			{
				return false;
			}

			_uploadService.ChangeValue(File.Id, "Waiting", false);
			_uploadService.ChangeId(File.Id, await response.Content.ReadFromJsonAsync<int>());
			return true;
		}

		private async Task UploadStage(UploadStatus File)
		{
			//Gets next data to send
			List<byte> nextDataRange = File.Data.Count > _byteDataSize ? File.Data.GetRange(0, _byteDataSize) : File.Data;

			//Uploads data to server
			HttpResponseMessage success = await _httpClient.PostAsJsonAsync<UploadData>($"api/Upload/Upload", new UploadData() { Id = File.Id, DataIndex = File.CurrentDataIndex, Data = nextDataRange });

			if (success is null || !success.IsSuccessStatusCode)
			{
				return;
			}

			//Delete data from file
			File.Data.RemoveRange(0, nextDataRange.Count());

			//Updates percentage
			float percentage = ((float)File.Data.Count() / (float)File.TotalDataLength) * 100f;
			File.Percentage = (int)(100f - percentage);

			_uploadService.IncrementDataIndex(File.Id);
		}

		private async void FinishFileUpload(UploadStatus File)
		{
			//Sends message to server to save the file in the right place
			await _httpClient.PostAsJsonAsync<UploadFinish>($"api/Upload/Finish", new UploadFinish() { Id = File.Id, PathCode = 1, TotalByteSize = File.TotalDataLength});

			File.Percentage = 100;

			UpdateWaitTime();
		}

		private async void CancelFileUpload(UploadStatus File)
		{
			//Asks server to cancel upload
			HttpResponseMessage? response = null;
			try
			{
				response = await _httpClient.PostAsJsonAsync<int>($"api/Upload/Cancel", File.Id);
			}
			catch
			{
				
			}

			if (response is null || !response.IsSuccessStatusCode || await response.Content.ReadFromJsonAsync<bool>() == false)
			{
				return;
			}

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
