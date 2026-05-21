using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models.Security;

public enum PermissionType
{
	None = 0,
	Inherited = 1,
	View = 2,
	Tag = 4,
	Edit = 8
}
