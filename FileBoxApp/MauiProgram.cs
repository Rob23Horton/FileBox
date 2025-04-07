using DatabaseConnector.Services;
using SQLitePCL;
using Microsoft.Extensions.Logging;

namespace FileBoxApp
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			Batteries.Init();
			Batteries_V2.Init();

			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();

			Directory.SetCurrentDirectory(FileSystem.AppDataDirectory);

			//Adds database connector using FileBox.StartUp
			builder.Services.AddSingleton<IDatabaseConnector>(FileBox.FileBox.StartUp());

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
