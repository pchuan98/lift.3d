using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Lift.Helix.Attach;

public class InfoAttach
{
    // 颜色
    public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
        "Foreground", typeof(Brush), typeof(InfoAttach), new FrameworkPropertyMetadata(Brushes.Black,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetForeground(DependencyObject element, Brush value)
        => element.SetValue(ForegroundProperty, value);

    public static Brush GetForeground(DependencyObject element)
        => (Brush) element.GetValue(ForegroundProperty);

    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
        "Background", typeof(Brush), typeof(InfoAttach), new FrameworkPropertyMetadata(Brushes.Transparent,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetBackground(DependencyObject element, Brush value)
        => element.SetValue(BackgroundProperty, value);

    public static Brush GetBackground(DependencyObject element)
        => (Brush) element.GetValue(BackgroundProperty);


    // info content
    public static readonly DependencyProperty ShowFpsProperty = DependencyProperty.RegisterAttached(
        "ShowFps", typeof(bool), typeof(InfoAttach), new FrameworkPropertyMetadata(false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetShowFps(DependencyObject element, bool value)
        => element.SetValue(ShowFpsProperty, value);

    public static bool GetShowFps(DependencyObject element)
        => (bool) element.GetValue(ShowFpsProperty);


    public static readonly DependencyProperty ShowOthersProperty = DependencyProperty.RegisterAttached(
        "ShowOthers", typeof(bool), typeof(InfoAttach), new FrameworkPropertyMetadata(false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static void SetShowOthers(DependencyObject element, bool value)
        => element.SetValue(ShowOthersProperty, value);

    public static bool GetShowOthers(DependencyObject element)
        => (bool) element.GetValue(ShowOthersProperty);
}
