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
	}
}
