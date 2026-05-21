using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models.Search;

public class SearchFolder : Folder
{
	private string _excludeFiles = string.Empty;
	/// <summary>
	///  This contains files that are ignored when scanned
	///   Eg - bin, 20% (2026, 2007), *.exe & *
	/// </summary>
	public List<string> ExcludeFiles
	{
		get => _excludeFiles.Split(',').ToList();
		set
		{
			_excludeFiles = string.Join(',', value);
		}
	}
	public string ExcludeFilesString
	{
		set => _excludeFiles = value;
	}


	private string _excludeFolders = string.Empty;
	/// <summary>
	///  This contains folders that are ignored when scanned
	/// </summary>
	public List<string> ExcludeFolders
	{
		get => _excludeFolders.Split(',').ToList();
		set
		{
			_excludeFolders = string.Join(',', value);
		}
	}
	public string ExcludeFoldersString
	{
		set => _excludeFolders = value;
	}
}
