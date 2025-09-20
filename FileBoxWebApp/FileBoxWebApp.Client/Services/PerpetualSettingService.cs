namespace FileBoxWebApp.Client.Services
{
	public class PerpetualSettingService
	{

		private Dictionary<string, object> _settings { get; set; }
		public PerpetualSettingService()
		{
			_settings = new Dictionary<string, object>();
		}

		public void SetSettingValue(string Key, object Value)
		{
			_settings[Key] = Value;
		}

		public object? GetSettingValue(string Key)
		{
			object? value = null;
			_settings.TryGetValue(Key, out value);

			return value;
		}

	}
}
