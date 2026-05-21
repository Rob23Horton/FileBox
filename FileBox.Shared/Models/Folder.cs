using FileBox.Shared.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models;

public class Folder
{
	public int Id { get; set; }
	public bool IsPrivate { get; set; } //Private in being only for that person. Might delete later
	public int? ParentFolderCode { get; set; } = null;
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
		//Name is required so it can be shown to the user
	public PermissionType Permission { get; set; } = PermissionType.None;
}

