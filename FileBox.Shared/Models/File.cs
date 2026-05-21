using FileBox.Shared.Models.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models;

public class File
{
	public int Id { get; set; }
	public ItemType Type { get; set; }
	private string _name;
	public required string Name
	{
		get => _name;
		set
		{
			if (value.Length > 120)
				throw new ArgumentException("Name cannot be longer than 120 characters");
			_name = value;
		}
	}
	public int FolderCode { get; set; }
	private string _extension;
	public required string Extension
	{
		get => _extension;
		set
		{
			if (value.Length > 7)
				throw new ArgumentException("Extension cannot be longer than 7 characters");
			_extension = value;
		}
	}
}
