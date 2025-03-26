using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.ConfigModels
{
	internal class AppSetttingsConfig
	{
		public DatabaseSettings DatabaseSettings { get; set; }
	}

	internal class DatabaseSettings
	{
		public string DatabaseType { get; set; }
		public string? ConnectionString { get; set; } //Can be null for Sqlite DBs
	}
}
