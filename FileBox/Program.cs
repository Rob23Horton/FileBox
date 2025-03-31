using DatabaseConnector.Services;
using DatabaseConnector.Models;
using FileBox.ConfigModels;
using System.Text.Json;
using FileBox.Shared.Models;
using System.Reflection;
using FileBox.Interfaces;
using FileBox.Repositories;
using System.Net.Http.Headers;

//Testing
IDatabaseConnector _databaseConnector = StartUp();

ITagRepository tagRepository = new TagRepository(_databaseConnector);
IPathRepository pathRepository = new PathRepository(_databaseConnector);
IFileRepository fileRepository = new FileRepository(_databaseConnector);


static IDatabaseConnector StartUp()
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

		//Gets tables config
		List<Table> Tables = GetDatabaseTables();

		//Creates tables if they don't already exist
		CreateTables(DatabaseConnector, Tables);
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

	Console.WriteLine("FileBox Started!");

	//From this point the DataBase connection is setup

	return DatabaseConnector;
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

static List<Table> GetDatabaseTables()
{
	string text = File.ReadAllText("tablesconfig.json");
	return JsonSerializer.Deserialize<List<Table>>(text);
}

#endregion

#region Sqlite Database
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

static void CreateTables(DatabaseConnector.Services.IDatabaseConnector Connection, List<Table> Tables)
{
	foreach (Table table in Tables)
	{
		Type? type = Type.GetType($"FileBox.Shared.Models.{table.CreatorClass}, Filebox.Shared", false);

		if (type is null)
		{
			Console.WriteLine($"Error trying to load creator class {table.CreatorClass} for table {table.Name}!");
			continue;
		}

		//Checks if the table is already created
		try
		{
			// Get the method from IDatabaseConnector, name and parameter(Wouldn't get it without this)
			MethodInfo method = typeof(IDatabaseConnector).GetMethod("Select", new Type[] { typeof(Select) })!;

			// Make the method generic with the specific type of the creator class
			MethodInfo genericMethod = method.MakeGenericMethod(type);

			// Invoke the generic method with using Connection as its base
			//Not the best due to getting all the data in the table for no reason
			object result = genericMethod.Invoke(Connection, new object[] { new Select() })!;

		}
		catch
		{
			//Creates the table in the database
			MethodInfo method = typeof(IDatabaseConnector).GetMethod("CreateTable")!;
			MethodInfo genericMethod = method.MakeGenericMethod(type);
			genericMethod.Invoke(Connection, new object[] { });
		}
	}
}

#endregion