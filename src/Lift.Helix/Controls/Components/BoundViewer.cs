using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using Lift.Helix.Extensions;

namespace Lift.Helix.Controls.Componets;

public class BoundViewer : ModelVisual3D
{
    private const double Zero = 0;

    private bool _isXLock = false;
    private bool _isYLock = false;
    private bool _isZLock = false;
    private bool _isBoundLock = false;

    private readonly ModelVisual3D _xAxesVisual;
    private readonly ModelVisual3D _yAxesVisual;
    private readonly ModelVisual3D _zAxesVisual;
    private readonly ModelVisual3D _boxVisual;

    public BoundViewer()
    {
        _xAxesVisual = new ModelVisual3D();
        _yAxesVisual = new ModelVisual3D();
        _zAxesVisual = new ModelVisual3D();
        _boxVisual = new ModelVisual3D();

        Children.Add(_xAxesVisual);
        Children.Add(_yAxesVisual);
        Children.Add(_zAxesVisual);
        Children.Add(_boxVisual);
    }

    #region Bound Property

    private static void OnBoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;

        viewer._isXLock = true;
        viewer._isYLock = true;
        viewer._isZLock = true;
        viewer._isBoundLock = true;

        var size = viewer.BoundSize;

        var max = Math.Max(viewer.BoundSize.SizeX, viewer.BoundSize.SizeY);
        max = Math.Max(max, viewer.BoundSize.SizeZ);

        // adjust some size by bound parms
        viewer.XTitleSize = max * 0.01;
        viewer.YTitleSize = max * 0.01;
        viewer.ZTitleSize = max * 0.01;

        viewer.XInterval = viewer.BoundSize.SizeX / 10;
        viewer.YInterval = viewer.BoundSize.SizeY / 10;
        viewer.ZInterval = viewer.BoundSize.SizeZ / 10;

        viewer.TickSize = max * 0.01;
        viewer.SubTickSize = max * 0.005;
        viewer.TickDiameter = max * 0.0005;

        viewer.BoundDiameter = max * 0.0005;

        viewer.RefreshBound(viewer);
        viewer.RefreshXAxes(viewer);
        viewer.RefreshYAxes(viewer);
        viewer.RefreshZAxes(viewer);

        viewer._isXLock = false;
        viewer._isYLock = false;
        viewer._isZLock = false;
        viewer._isBoundLock = false;

        var parent = viewer.FindParent<HelixViewport3D>();
        if (parent is null) return;
        parent.ResetCamera();
    }

    private static void OnBoundChangedTiny(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;

        if (viewer._isBoundLock) return;

        viewer.RefreshBound(viewer);
    }

    private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;

        var points = viewer.Points;

        var minX = points.Min(p => p.X);
        var maxX = points.Max(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxY = points.Max(p => p.Y);
        var minZ = points.Min(p => p.Z);
        var maxZ = points.Max(p => p.Z);

        var bound = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);
        viewer.BoundSize = bound;
    }

    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        nameof(Points), typeof(Point3D[]), typeof(BoundViewer), new FrameworkPropertyMetadata(default(Point3D[]),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPointsChanged));

    public Point3D[] Points
    {
        get => (Point3D[]) GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public static readonly DependencyProperty BoundSizeProperty = DependencyProperty.Register(
        nameof(BoundSize), typeof(Rect3D), typeof(BoundViewer), new FrameworkPropertyMetadata(default(Rect3D),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundChanged));

    public Rect3D BoundSize
    {
        get => (Rect3D) GetValue(BoundSizeProperty);
        set => SetValue(BoundSizeProperty, value);
    }

    public static readonly DependencyProperty BoundDiameterProperty = DependencyProperty.Register(
        nameof(BoundDiameter), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata((double)0,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundChangedTiny));

    public double BoundDiameter
    {
        get => (double) GetValue(BoundDiameterProperty);
        set => SetValue(BoundDiameterProperty, value);
    }

    public static readonly DependencyProperty BoundBrushProperty = DependencyProperty.Register(
        nameof(BoundBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.Black,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundChangedTiny));


    public Brush BoundBrush
    {
        get => (Brush) GetValue(BoundBrushProperty);
        set => SetValue(BoundBrushProperty, value);
    }

    #endregion

    #region Axes

    private static void OnAxesValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        if (viewer._isXLock && viewer is { _isYLock: true, _isZLock: true }) return;

        viewer.RefreshBound(viewer);
        viewer.RefreshXAxes(viewer);
        viewer.RefreshYAxes(viewer);
        viewer.RefreshZAxes(viewer);
    }

    public static readonly DependencyProperty TickSizeProperty = DependencyProperty.Register(
        nameof(TickSize), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAxesValueChanged));

    public double TickSize
    {
        get => (double) GetValue(TickSizeProperty);
        set => SetValue(TickSizeProperty, value);
    }

    public static readonly DependencyProperty SubTickSizeProperty = DependencyProperty.Register(
        nameof(SubTickSize), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAxesValueChanged));


    public double SubTickSize
    {
        get => (double) GetValue(SubTickSizeProperty);
        set => SetValue(SubTickSizeProperty, value);
    }

    public static readonly DependencyProperty TickDiameterProperty = DependencyProperty.Register(
        nameof(TickDiameter), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAxesValueChanged));

    public double TickDiameter
    {
        get => (double) GetValue(TickDiameterProperty);
        set => SetValue(TickDiameterProperty, value);
    }

    #region X

    private static void OnXValueChangedTiny(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        viewer.RefreshXAxes(viewer);
    }

    private static void OnXValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        if (viewer._isXLock) return;
        viewer.RefreshXAxes(viewer);
    }

    public static readonly DependencyProperty XTitleProperty = DependencyProperty.Register(
        nameof(XTitle), typeof(string), typeof(BoundViewer), new FrameworkPropertyMetadata("X-Axes", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChangedTiny));

    public string XTitle
    {
        get => (string) GetValue(XTitleProperty);
        set => SetValue(XTitleProperty, value);
    }

    public static readonly DependencyProperty XTitleBrushProperty = DependencyProperty.Register(
        nameof(XTitleBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChangedTiny));

    public Brush XTitleBrush
    {
        get => (Brush) GetValue(XTitleBrushProperty);
        set => SetValue(XTitleBrushProperty, value);
    }

    public static readonly DependencyProperty XTitleSizeProperty = DependencyProperty.Register(
        nameof(XTitleSize), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChanged));

    public double XTitleSize
    {
        get => (double) GetValue(XTitleSizeProperty);
        set => SetValue(XTitleSizeProperty, value);
    }

    public static readonly DependencyProperty XTickBrushProperty = DependencyProperty.Register(
        nameof(XTickBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChangedTiny));

    public Brush XTickBrush
    {
        get => (Brush) GetValue(XTickBrushProperty);
        set => SetValue(XTickBrushProperty, value);
    }

    public static readonly DependencyProperty XIntervalProperty = DependencyProperty.Register(
        nameof(XInterval), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChanged));

    public double XInterval
    {
        get => (double) GetValue(XIntervalProperty);
        set => SetValue(XIntervalProperty, value);
    }

    public static readonly DependencyProperty ShowXTickProperty = DependencyProperty.Register(
        nameof(ShowXTick), typeof(bool), typeof(BoundViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChangedTiny));

    public bool ShowXTick
    {
        get => (bool) GetValue(ShowXTickProperty);
        set => SetValue(ShowXTickProperty, value);
    }

    public static readonly DependencyProperty ShowXSubTickProperty = DependencyProperty.Register(
        nameof(ShowXSubTick), typeof(bool), typeof(BoundViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnXValueChangedTiny));

    public bool ShowXSubTick
    {
        get => (bool) GetValue(ShowXSubTickProperty);
        set => SetValue(ShowXSubTickProperty, value);
    }

    #endregion

    #region Y

    private static void OnYValueChangedTiny(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        viewer.RefreshYAxes(viewer);
    }

    private static void OnYValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        if (viewer._isYLock) return;
        viewer.RefreshYAxes(viewer);
    }

    public static readonly DependencyProperty YTitleProperty = DependencyProperty.Register(
        nameof(YTitle), typeof(string), typeof(BoundViewer), new FrameworkPropertyMetadata("Y-Axes", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChangedTiny));

    public string YTitle
    {
        get => (string) GetValue(YTitleProperty);
        set => SetValue(YTitleProperty, value);
    }

    public static readonly DependencyProperty YTitleBrushProperty = DependencyProperty.Register(
        nameof(YTitleBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChangedTiny));

    public Brush YTitleBrush
    {
        get => (Brush) GetValue(YTitleBrushProperty);
        set => SetValue(YTitleBrushProperty, value);
    }

    public static readonly DependencyProperty YTitleSizeProperty = DependencyProperty.Register(
        nameof(YTitleSize), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChanged));

    public double YTitleSize
    {
        get => (double) GetValue(YTitleSizeProperty);
        set => SetValue(YTitleSizeProperty, value);
    }

    public static readonly DependencyProperty YTickBrushProperty = DependencyProperty.Register(
        nameof(YTickBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChangedTiny));

    public Brush YTickBrush
    {
        get => (Brush) GetValue(YTickBrushProperty);
        set => SetValue(YTickBrushProperty, value);
    }

    public static readonly DependencyProperty YIntervalProperty = DependencyProperty.Register(
        nameof(YInterval), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChanged));

    public double YInterval
    {
        get => (double) GetValue(YIntervalProperty);
        set => SetValue(YIntervalProperty, value);
    }

    public static readonly DependencyProperty ShowYTickProperty = DependencyProperty.Register(
        nameof(ShowYTick), typeof(bool), typeof(BoundViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChangedTiny));

    public bool ShowYTick
    {
        get => (bool) GetValue(ShowYTickProperty);
        set => SetValue(ShowYTickProperty, value);
    }

    public static readonly DependencyProperty ShowYSubTickProperty = DependencyProperty.Register(
        nameof(ShowYSubTick), typeof(bool), typeof(BoundViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnYValueChangedTiny));

    public bool ShowYSubTick
    {
        get => (bool) GetValue(ShowYSubTickProperty);
        set => SetValue(ShowYSubTickProperty, value);
    }

    #endregion

    #region Z

    private static void OnZValueChangedTiny(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        viewer.RefreshZAxes(viewer);
    }

    private static void OnZValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BoundViewer viewer) return;
        if (viewer._isZLock) return;
        viewer.RefreshZAxes(viewer);
    }

    public static readonly DependencyProperty ZTitleProperty = DependencyProperty.Register(
        nameof(ZTitle), typeof(string), typeof(BoundViewer), new FrameworkPropertyMetadata("Z-Axes", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChangedTiny));

    public string ZTitle
    {
        get => (string) GetValue(ZTitleProperty);
        set => SetValue(ZTitleProperty, value);
    }

    public static readonly DependencyProperty ZTitleBrushProperty = DependencyProperty.Register(
        nameof(ZTitleBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChangedTiny));

    public Brush ZTitleBrush
    {
        get => (Brush) GetValue(ZTitleBrushProperty);
        set => SetValue(ZTitleBrushProperty, value);
    }

    public static readonly DependencyProperty ZTitleSizeProperty = DependencyProperty.Register(
        nameof(ZTitleSize), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChanged));

    public double ZTitleSize
    {
        get => (double) GetValue(ZTitleSizeProperty);
        set => SetValue(ZTitleSizeProperty, value);
    }

    public static readonly DependencyProperty ZTickBrushProperty = DependencyProperty.Register(
        nameof(ZTickBrush), typeof(Brush), typeof(BoundViewer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChangedTiny));

    public Brush ZTickBrush
    {
        get => (Brush) GetValue(ZTickBrushProperty);
        set => SetValue(ZTickBrushProperty, value);
    }

    public static readonly DependencyProperty ZIntervalProperty = DependencyProperty.Register(
        nameof(ZInterval), typeof(double), typeof(BoundViewer), new FrameworkPropertyMetadata(Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChanged));

    public double ZInterval
    {
        get => (double) GetValue(ZIntervalProperty);
        set => SetValue(ZIntervalProperty, value);
    }

    public static readonly DependencyProperty ShowZTickProperty = DependencyProperty.Register(
        nameof(ShowZTick), typeof(bool), typeof(BoundViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChangedTiny));

    public bool ShowZTick
    {
        get => (bool) GetValue(ShowZTickProperty);
        set => SetValue(ShowZTickProperty, value);
    }

    public static readonly DependencyProperty ShowZSubTickProperty = DependencyProperty.Register(
        nameof(ShowZSubTick), typeof(bool), typeof(BoundViewer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZValueChangedTiny));

    public bool ShowZSubTick
    {
        get => (bool) GetValue(ShowZSubTickProperty);
        set => SetValue(ShowZSubTickProperty, value);
    }

    #endregion

    #endregion

    #region refresh

    void RefreshBound(BoundViewer viewer)
    {
        var boxBuilder = new MeshBuilder();

        // note : 这里会自动设置，应该处理变量变化的过程
        boxBuilder.AddBoundingBox(viewer.BoundSize, viewer.BoundDiameter);
        //boxBuilder.AddBox(viewer.BoundSize,BoxFaces.PositiveX);

        var box = new GeometryModel3D(boxBuilder.ToMesh(), MaterialHelper.CreateMaterial(viewer.BoundBrush));

        viewer._boxVisual.Content = box;
    }

    void RefreshXAxes(BoundViewer viewer)
    {
        var model = new Model3DGroup();

        var count = (int) Math.Ceiling((viewer.BoundSize.SizeX / viewer.XInterval));
        var x = viewer.BoundSize.X;
        var y = viewer.BoundSize.Y;
        var z = viewer.BoundSize.Z;

        var tickSize = viewer.TickSize;
        var subTickSize = viewer.SubTickSize;

        var subInterval = viewer.XInterval / 5;

        var xMax = viewer.BoundSize.SizeX + x;

        if (count == 0) return;

        var axesBuilder = new MeshBuilder();

        for (var i = 0; i < count; i++)
        {
            var start = x + i * viewer.XInterval;

            // tick
            if (ShowXTick && i != 0)
            {
                axesBuilder.AddCylinder(new Point3D(start, y, z), new Point3D(start, y, z + tickSize), viewer.TickDiameter, 6);
                axesBuilder.AddCylinder(new Point3D(start, y, z), new Point3D(start, y + tickSize, z), viewer.TickDiameter, 6);

                var label = TextCreator.CreateTextLabelModel3D($"{start:F2}", viewer.XTickBrush, true, viewer.SubTickSize,
                    new Point3D(start, y - viewer.SubTickSize * 4, z), new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0));
                model.Children.Add(label);
            }

            // subtick
            if (!ShowXSubTick) continue;

            for (var j = 1; j < 5; j++)
            {
                var subStart = start + subInterval * j;
                if (subStart > xMax) continue;

                axesBuilder.AddCylinder(new Point3D(subStart, y, z), new Point3D(subStart, y, z + subTickSize), viewer.TickDiameter, 6);
                axesBuilder.AddCylinder(new Point3D(subStart, y, z), new Point3D(subStart, y + subTickSize, z), viewer.TickDiameter, 6);
            }
        }

        // label
        model.Children.Add(TextCreator.CreateTextLabelModel3D(viewer.XTitle, viewer.XTitleBrush, true, viewer.XTitleSize,
            new Point3D((x + xMax) * 0.5, y - viewer.XTitleSize * 6, z), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0)));

        // ticks
        model.Children.Add(new GeometryModel3D(axesBuilder.ToMesh(), MaterialHelper.CreateMaterial(viewer.XTickBrush)));

        viewer._xAxesVisual.Content = model;
    }

    void RefreshYAxes(BoundViewer viewer)
    {
        var model = new Model3DGroup();

        var count = (int) Math.Ceiling((viewer.BoundSize.SizeY / viewer.YInterval));

        var x = viewer.BoundSize.X;
        var y = viewer.BoundSize.Y;
        var z = viewer.BoundSize.Z;

        var tickSize = viewer.TickSize;
        var subTickSize = viewer.SubTickSize;

        var subInterval = viewer.YInterval / 5;

        var yMax = viewer.BoundSize.SizeY + y;

        if (count == 0) return;

        var axesBuilder = new MeshBuilder();

        for (var i = 0; i < count; i++)
        {
            var start = y + i * viewer.YInterval;

            // tick
            if (ShowYTick && i != 0)
            {
                axesBuilder.AddCylinder(new Point3D(x, start, z), new Point3D(x, start, z + tickSize), viewer.TickDiameter, 6);
                axesBuilder.AddCylinder(new Point3D(x, start, z), new Point3D(x + tickSize, start, z), viewer.TickDiameter, 6);

                var label = TextCreator.CreateTextLabelModel3D($"{start:F2}", viewer.YTickBrush, true, viewer.SubTickSize,
                    new Point3D(x - viewer.SubTickSize * 4, start, z), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0));
                model.Children.Add(label);
            }

            // subtick
            if (!ShowYSubTick) continue;

            for (var j = 1; j < 5; j++)
            {
                var subStart = start + subInterval * j;
                if (subStart > yMax) continue;

                axesBuilder.AddCylinder(new Point3D(x, subStart, z), new Point3D(x, subStart, z + subTickSize), viewer.TickDiameter, 6);
                axesBuilder.AddCylinder(new Point3D(x, subStart, z), new Point3D(x + subTickSize, subStart, z), viewer.TickDiameter, 6);
            }
        }

        // label
        model.Children.Add(TextCreator.CreateTextLabelModel3D(viewer.YTitle, viewer.YTitleBrush, true, viewer.YTitleSize,
            new Point3D(x - viewer.YTitleSize * 6, (y + yMax) / 2, z), new Vector3D(0, 1, 0), new Vector3D(-1, 0, 0)));

        // ticks
        model.Children.Add(new GeometryModel3D(axesBuilder.ToMesh(), MaterialHelper.CreateMaterial(viewer.YTickBrush)));

        viewer._yAxesVisual.Content = model;
    }

    void RefreshZAxes(BoundViewer viewer)
    {
        var model = new Model3DGroup();

        var count = (int) Math.Ceiling((viewer.BoundSize.SizeZ / viewer.ZInterval));

        var x = viewer.BoundSize.X;
        var y = viewer.BoundSize.Y;
        var z = viewer.BoundSize.Z;

        var tickSize = viewer.TickSize;
        var subTickSize = viewer.SubTickSize;

        var subInterval = viewer.ZInterval / 5;

        var zMax = viewer.BoundSize.SizeZ + z;

        if (count == 0) return;

        var axesBuilder = new MeshBuilder();

        for (var i = 0; i < count; i++)
        {
            var start = z + i * viewer.ZInterval;

            // tick
            if (ShowZTick && i != 0)
            {
                axesBuilder.AddCylinder(new Point3D(x, y, start), new Point3D(x + tickSize, y, start), viewer.TickDiameter, 6);
                axesBuilder.AddCylinder(new Point3D(x, y, start), new Point3D(x, y + tickSize, start), viewer.TickDiameter, 6);

                var label = TextCreator.CreateTextLabelModel3D($"{start:F2}", viewer.ZTickBrush, true, viewer.SubTickSize,
                    new Point3D(x - viewer.SubTickSize * 4, y, start), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1));
                model.Children.Add(label);
            }

            // subtick
            if (!ShowZSubTick) continue;

            for (var j = 1; j < 5; j++)
            {
                var subStart = start + subInterval * j;
                if (subStart > zMax) continue;

                axesBuilder.AddCylinder(new Point3D(x, y, subStart), new Point3D(x + subTickSize, y, subStart), viewer.TickDiameter, 6);
                axesBuilder.AddCylinder(new Point3D(x, y, subStart), new Point3D(x, y + subTickSize, subStart), viewer.TickDiameter, 6);
            }
        }

        // label
        model.Children.Add(TextCreator.CreateTextLabelModel3D(viewer.ZTitle, viewer.ZTitleBrush, true, viewer.ZTitleSize,
            new Point3D(x - viewer.ZTitleSize * 6, y, (z + zMax) / 2), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0)));

        // ticks
        model.Children.Add(new GeometryModel3D(axesBuilder.ToMesh(), MaterialHelper.CreateMaterial(viewer.ZTickBrush)));

        viewer._zAxesVisual.Content = model;
    }

    #endregion
}
