using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Lift.Helix.Core;

public enum Colormap
{
    Red,
    Green,
    Blue,

    Rainbow,
}

public static class Colormaps
{
    public static Brush QuickGenerateGradient(Color color1, Color color2)
        => new LinearGradientBrush(color1, color2, new Point(0, 0), new Point(0, 1));

    public static Brush Green
        => QuickGenerateGradient(new Color() { A = 0, B = 0, G = 255, R = 0 },
            new Color() { A = 255, B = 0, G = 255, R = 0 });

    public static Brush Red
        => QuickGenerateGradient(new Color() { A = 0, B = 0, G = 0, R = 255 },
            new Color() { A = 255, B = 0, G = 0, R = 255 });

    public static Brush Blue
        => QuickGenerateGradient(new Color() { A = 0, B = 255, G = 0, R = 0 },
            new Color() { A = 255, B = 255, G = 0, R = 0 });

    public static Brush Rainbow(double alpha)
    {
        // 创建一个线性渐变笔刷
        var brush = new LinearGradientBrush();

        var alp = (byte) (alpha * 255.0);

        // 添加多个渐变色停止点以模拟彩虹
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 255, 0, 0), 0));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 255, 165, 0), 0.17));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 255, 255, 0), 0.33));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 0, 128, 0), 0.5));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 0, 0, 255), 0.67));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 75, 0, 130), 0.83));
        brush.GradientStops.Add(new GradientStop(Color.FromArgb(alp, 148, 0, 211), 1.0));

        return brush;
    }

    public static Brush Converter(Colormap map, double alpha = 1) => map switch
    {
        Colormap.Green => Colormaps.Green,
        Colormap.Red => Colormaps.Red,
        Colormap.Blue => Colormaps.Blue,
        Colormap.Rainbow => Colormaps.Rainbow(alpha),

        _ => throw new ArgumentOutOfRangeException()
    };
}

