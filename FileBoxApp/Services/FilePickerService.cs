using Microsoft.Maui.Storage;

namespace FileBoxApp.Services
{
	public class FilePickerService
	{
		public async Task<string> PickFileAsync()
		{
			try
			{
				var result = await FilePicker.PickAsync();
				if (result is null)
				{
					return "";
				}
				return result.FullPath;
			}
			catch
			{
				return "";
			}
		}

		public async Task<List<string>> PickMultiFilesAsync()
		{
			try
			{
				IEnumerable<FileResult> result = await FilePicker.PickMultipleAsync();

				List<string> files = result.Select(r => r.FullPath).ToList();

				return files;
			}
			catch
			{
				return new List<string>();
			}
		}
	}
}
