using System;
using System.Runtime.InteropServices;

namespace SmartNet.WinApi
{
	public class Kernel32
	{
		[DllImport("kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string dllPath);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		internal static extern bool FreeLibrary(IntPtr hModule);
	}
}
