using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AiPrompt.Helpers;

public static class WinApi
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GlobalFree(IntPtr hMem);

    [DllImport("user32.dll")]
    public static extern bool EmptyClipboard();

    public const uint CF_TEXT = 1;
    public const uint CF_UNICODETEXT = 13;
}
