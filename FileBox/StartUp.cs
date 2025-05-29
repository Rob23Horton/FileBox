using DatabaseConnector.Services;
using DatabaseConnector.Models;
using FileBox.ConfigModels;
using System.Text.Json;
using System.Reflection;
using FileBox.Interfaces;
using FileBox.Repositories;
using FileBox.Shared.Models;

namespace FileBox
{
	public static class FileBox
	{
		static public IDatabaseConnector StartUp(string? SqliteDirectiory)
		{
			DatabaseType DbType = GetDatabaseType();

			DatabaseConnector.Services.IDatabaseConnector DatabaseConnector;

			if (DbType == DatabaseType.MSqlite || DbType == DatabaseType.Sqlite) 
			{
				if (SqliteDirectiory == null)
				{
					throw new Exception("SqliteDirectory must be set to create a Sqlite connection.");
				}

				DatabaseConnector = new DatabaseConnector.Services.DatabaseConnector(DbType, $"Data Source={SqliteDirectiory}\\FileBoxDatabase.db");

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

			return DatabaseConnector;
		}

	#region App Settings
		static AppSetttingsConfig GetAppSettings()
		{
			try
			{
				string text = File.ReadAllText("appsettings.json");
				return JsonSerializer.Deserialize<AppSetttingsConfig>(text);
			}
			catch
			{
				return new AppSetttingsConfig() { DatabaseSettings = new DatabaseSettings() { DatabaseType = "MSqlite" } };
			}
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
			return new List<Table>() {
				new Table() { Name = "tblFile", Type = typeof(FileBoxFile)},
				new Table() { Name = "tblTag", Type = typeof(Tag)},
				new Table() { Name = "tblPath", Type = typeof(FileBoxPath)},
				new Table() { Name = "tblInfoValue", Type = typeof(InfoValue)},
				new Table() { Name = "tblFileTag", Type = typeof(FileTag)}
				};
		}

	#endregion

	#region Sqlite Database

		static void CreateTables(DatabaseConnector.Services.IDatabaseConnector Connection, List<Table> Tables)
		{
			foreach (Table table in Tables)
			{

				//Checks if the table is already created
				try
				{
					// Get the method from IDatabaseConnector, name and parameter(Wouldn't get it without this)
					MethodInfo method = typeof(IDatabaseConnector).GetMethod("Select", new Type[] { typeof(Select) })!;

					// Make the method generic with the specific type of the creator class
					MethodInfo genericMethod = method.MakeGenericMethod(table.Type);

					// Invoke the generic method with using Connection as its base
					//Not the best due to getting all the data in the table for no reason
					object result = genericMethod.Invoke(Connection, new object[] { new Select() })!;

				}
				catch
				{
					//Creates the table in the database
					MethodInfo method = typeof(IDatabaseConnector).GetMethod("CreateTable")!;
					MethodInfo genericMethod = method.MakeGenericMethod(table.Type);
					genericMethod.Invoke(Connection, new object[] { });
				}
			}
		}

	#endregion
	}
}