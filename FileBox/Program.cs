using FileBox.Shared.Models;
using DatabaseConnector.Services;
using DatabaseConnector.Models;
using FileBox.ConfigModels;
using System.Text.Json;


StartUp();


static void StartUp()
{

	Console.WriteLine("Starting FileBox!");

	DatabaseType DbType = GetDatabaseType();

	DatabaseConnector.Services.IDatabaseConnector DatabaseConnector;

	if (DbType == DatabaseType.Sqlite)
	{
		//Gets the path to the database as well as creates the path if it doesn't already exist
		string SqliteLocation = GetSqliteDatabaseLocation();
		Directory.CreateDirectory(SqliteLocation);

		DatabaseConnector = new DatabaseConnector.Services.DatabaseConnector(DatabaseType.Sqlite, $"Data Source={SqliteLocation}\\FileBoxDatabase.db");

		//TODO - Create tables if they don't already exist
	}

	else //Creates databases that are not local files (MySql, SqlServer, etc)
	{
		string? ConnectionString = GetConnectionString();

		if (ConnectionString is null)
		{
			throw new Exception("Connection String cannot be null. Please enter a connection string in appsettings.json.");
		}

		DatabaseConnector = new DatabaseConnector.Services.DatabaseConnector(DbType, ConnectionString);

	}


	//From this point the DataBase connection is setup

	List<FileBoxFile> files = DatabaseConnector.Select<FileBoxFile>(new Select());

	Console.WriteLine(files.Count());

}

#region App Settings
static AppSetttingsConfig GetAppSettings()
{
	string text = File.ReadAllText("appsettings.json");
	return JsonSerializer.Deserialize<AppSetttingsConfig>(text);
}
static DatabaseType GetDatabaseType()
{
	AppSetttingsConfig appSettings = GetAppSettings();

	return (DatabaseType)Enum.Parse(typeof(DatabaseType), appSettings.DatabaseSettings.DatabaseType);
}
static string? GetConnectionString()
{
	AppSetttingsConfig appSettings = GetAppSettings();

	return appSettings.DatabaseSettings.ConnectionString;
}
#endregion

#region Sqlite Database Settings
static string GetSqliteDatabaseLocation()
{
	string text = File.ReadAllText("filelocationsettings.json");
	FileLocationConfig fileLocations = JsonSerializer.Deserialize<FileLocationConfig>(text);

	if (OperatingSystem.IsWindows())
	{
		return fileLocations.Windows;
	}
	else if (OperatingSystem.IsLinux())
	{
		return fileLocations.Linux;
	}
	else
	{
		throw new Exception("Operating System is not supported! Please use Windows or Linux!");
	}


}
#endregion