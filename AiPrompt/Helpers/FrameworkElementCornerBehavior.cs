using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace AiPrompt.Helpers;

//public class FrameworkElementCornerBehavior : Behavior<FrameworkElement>
//{
//    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FrameworkElementCornerBehavior), new PropertyMetadata(new CornerRadius(0), OnCornerRadiusChanged));


//    protected override void OnAttached()
//    {
//        var r = new RectangleGeometry();
//        AssociatedObject.Clip = r;

//        BindingOperations.SetBinding(r, RectangleGeometry.RectProperty, new Binding()
//        {
//            Source = AssociatedObject,
//            Path = new PropertyPath("ActualWidth"),
//        });
//    }
//}