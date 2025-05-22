using DatabaseConnector.Services;
using SQLitePCL;
using Microsoft.Extensions.Logging;

using Blazored.LocalStorage;
using FileBoxApp.Services;
using FileBox.Interfaces;
using FileBox.Repositories;

using System;
using CommunityToolkit.Maui;


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
				.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();

			Directory.SetCurrentDirectory(FileSystem.AppDataDirectory);

			//Adds database connector using FileBox.StartUp
			string databaseLocation = OperatingSystem.IsWindows() ? "C:\\ProgramData\\FileBox" : FileSystem.AppDataDirectory;
			builder.Services.AddSingleton<IDatabaseConnector>(FileBox.FileBox.StartUp(databaseLocation));

			//Adds local storage service
			builder.Services.AddBlazoredLocalStorage();

			//Custom Services
			builder.Services.AddSingleton<FilePickerService>();
			builder.Services.AddScoped<IRecentFilesRepository, RecentFileRepository>();
			builder.Services.AddScoped<ILocalFileSystemService, LocalFileSystemService>();
			builder.Services.AddScoped<ISearchService, SearchService>();
			builder.Services.AddScoped<MetaDataService>();

			builder.Services.AddScoped<IFileRepository, FileRepository>();
			builder.Services.AddScoped<ITagRepository, TagRepository>();
			builder.Services.AddScoped<IPathRepository, PathRepository>();
			builder.Services.AddScoped<IFileInformationRepository, FileInformationRepository>();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
