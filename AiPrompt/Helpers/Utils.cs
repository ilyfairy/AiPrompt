using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AiPrompt.Helpers;

public static class Utils
{
    private static PresentationSource? presentationSource;
    public static (double XScale,double YScale) GetScale()
    {
        presentationSource ??= PresentationSource.FromVisual(Application.Current.MainWindow);

        if (presentationSource == null) return (1, 1);
        var scaleX = presentationSource.CompositionTarget.TransformToDevice.M11;
        var scaleY = presentationSource.CompositionTarget.TransformToDevice.M22;
        return (scaleX, scaleY);
    }
}
