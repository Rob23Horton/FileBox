namespace FileBox.Shared.Models
{
	public class FileLoad
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Data { get; set; }

		public FileLoad Clone()
		{
			return new FileLoad { Id = Id, Name = Name, Type = Type, Data = Data };
		}
	}
}
