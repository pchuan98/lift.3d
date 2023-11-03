using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lift.Helix.Controls.DX;

public class VolumeModel:Panel
{
    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        nameof(Points), typeof(float[]), typeof(VolumeModel), new PropertyMetadata(default(float[])));

    public float[] Points
    {
        get => (float[]) GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }
}
