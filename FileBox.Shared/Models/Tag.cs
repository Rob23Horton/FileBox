using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models;

public class Tag
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int? ParentTagCode {  get; set; } = null;
	public string Colour { get; set; }
}
