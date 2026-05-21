using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBox.Shared.Models;

public class ApiResponse<T>
{
	[MemberNotNullWhen(true, nameof(Value))]
	[MemberNotNullWhen(false, nameof(ErrorMessage))]
	public bool Success { get; set; }
	public T? Value { get; set; }
	public string? ErrorMessage { get; set; }
}
