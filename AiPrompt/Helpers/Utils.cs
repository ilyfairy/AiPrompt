using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AiPrompt.Helpers;

public static class Utils
{
    private static PresentationSource? presentationSource;
    public static (double XScale, double YScale) GetScale()
    {
        presentationSource ??= PresentationSource.FromVisual(Application.Current.MainWindow);

        if (presentationSource == null) return (1, 1);
        var scaleX = presentationSource.CompositionTarget.TransformToDevice.M11;
        var scaleY = presentationSource.CompositionTarget.TransformToDevice.M22;
        return (scaleX, scaleY);
    }
    public static void SetClipboardText(string text)
    {
        if (!WinApi.OpenClipboard(IntPtr.Zero))
        {
            return;
        }

        int size = text.Length * 2 + 2;

        IntPtr hData = WinApi.GlobalAlloc(0x0042, (nuint)(size));
        if (hData == 0)
        {
            WinApi.CloseClipboard();
            return;
        }

        IntPtr pData = WinApi.GlobalLock(hData);
        if (pData == 0)
        {
            WinApi.GlobalFree(hData);
            WinApi.CloseClipboard();
            return;
        }

        unsafe
        {
            fixed (char* p = text)
            {
                NativeMemory.Copy(p, (void*)pData, (nuint)size);
            }
        }
        //Marshal.Copy(text.ToCharArray(), 0, pData, text.Length);
        WinApi.GlobalUnlock(hData);

        WinApi.EmptyClipboard();
        if (WinApi.SetClipboardData(WinApi.CF_UNICODETEXT, pData) == 0)
        {
            WinApi.GlobalFree(hData);
            WinApi.CloseClipboard();
            return;
        }

        WinApi.CloseClipboard();
    }
}
