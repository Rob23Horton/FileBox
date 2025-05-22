using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileBox.Shared.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace FileBoxApp.Services
{
	public class MetaDataService
	{
		public List<InfoValue> GetMetaDataForFile(string FilePath)
		{
			try
			{

				List<MetadataExtractor.Directory> directory = ImageMetadataReader.ReadMetadata(FilePath).ToList();

				if (directory.Count == 0)
				{
					return new List<InfoValue>();
				}

				List<InfoValue> metaData = new List<InfoValue>();

				foreach (MetadataExtractor.Tag tag in directory[0].Tags)
				{
					if (tag.Description is null)
					{
						continue;
					}

					metaData.Add(new InfoValue()
					{
						ValueName = tag.Name,
						Value = tag.Description
					});
				}

				return metaData;
			}
			catch
			{
				return new List<InfoValue>();
			}
		}
	}
}
