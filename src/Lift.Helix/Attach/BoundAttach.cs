using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Lift.Helix.Attach;

/// <summary>
///     控制所有和bound相关的属性
/// </summary>
public class BoundAttach
{
    // 放置信息
    public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached(
        "Location", typeof(Rect3D), typeof(BoundAttach), new FrameworkPropertyMetadata(default(Rect3D),
            FrameworkPropertyMetadataOptions.Inherits));

    public static void SetLocation(DependencyObject element, Rect3D value)
        => element.SetValue(LocationProperty, value);

    public static Rect3D GetLocation(DependencyObject element)
        => (Rect3D) element.GetValue(LocationProperty);


    // 线宽
    public static readonly DependencyProperty DiameterProperty = DependencyProperty.RegisterAttached(
        "Diameter", typeof(double), typeof(BoundAttach), new FrameworkPropertyMetadata((double)0,
            FrameworkPropertyMetadataOptions.Inherits));

    public static void SetDiameter(DependencyObject element, double value)
        => element.SetValue(DiameterProperty, value);

    public static double GetDiameter(DependencyObject element)
        => (double) element.GetValue(DiameterProperty);

    // Color
    public static readonly DependencyProperty BrushProperty = DependencyProperty.RegisterAttached(
        "Brush", typeof(Brush), typeof(BoundAttach), new FrameworkPropertyMetadata(Brushes.Black,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetBrush(DependencyObject element, Brush value)
        => element.SetValue(BrushProperty, value);

    public static Brush GetBrush(DependencyObject element)
        => (Brush) element.GetValue(BrushProperty);


    // 是否显示
    public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached(
        "Visibility", typeof(Visibility), typeof(BoundAttach), new FrameworkPropertyMetadata(Visibility.Visible,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetVisibility(DependencyObject element, Visibility value)
        => element.SetValue(VisibilityProperty, value);

    public static Visibility GetVisibility(DependencyObject element)
        => (Visibility) element.GetValue(VisibilityProperty);

}


