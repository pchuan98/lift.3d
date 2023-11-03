using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lift.Helix.Attach;

public class ViewportAttach
{
    public static readonly DependencyProperty InfiniteSpinProperty = DependencyProperty.RegisterAttached(
        "InfiniteSpin", typeof(bool), typeof(ViewportAttach), new FrameworkPropertyMetadata(false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetInfiniteSpin(DependencyObject element, bool value)
        => element.SetValue(InfiniteSpinProperty, value);

    public static bool GetInfiniteSpin(DependencyObject element)
        => (bool) element.GetValue(InfiniteSpinProperty);

    public static readonly DependencyProperty RotationSensitivityProperty = DependencyProperty.RegisterAttached(
        "RotationSensitivity", typeof(double), typeof(ViewportAttach), new FrameworkPropertyMetadata((double)1,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetRotationSensitivity(DependencyObject element, double value)
        => element.SetValue(RotationSensitivityProperty, value);

    public static double GetRotationSensitivity(DependencyObject element)
        => (double) element.GetValue(RotationSensitivityProperty);
}
