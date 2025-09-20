namespace FileBoxWebApp.Models
{
	public class NotAllDataUploadedException : Exception
	{
		public NotAllDataUploadedException(List<int> MissingPackets)
		{
			this.MissingPackets = MissingPackets;
		}

		public List<int> MissingPackets { get; set; }
	}
}
