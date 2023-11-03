#define TestCode // CS1032, put before namespace
#undef TestCode

// note: 关于模型和模型之间因为各种原因产生的

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Lift.Helix.Attach;
using Lift.Helix.Controls.Componets;
using Lift.Helix.Core;

namespace Lift.Helix.Controls;

[TemplatePart(Name = Bound, Type = typeof(ModelVisual3D))]
[TemplatePart(Name = Axis, Type = typeof(ModelVisual3D))]
[TemplatePart(Name = Scatter, Type = typeof(ModelVisual3D))]
[TemplatePart(Name = Port, Type = typeof(HelixViewport3D))]
public class SimpleScatterViewer : Control
{
    private const string Bound = "PART_Bound";

    private const string Axis = "PART_Axis";

    private const string Scatter = "PART_Scatter";

    private const string Port = "PART_Port";

    private BoundViewer? _bound = null;

    private ModelVisual3D? _scatter = null;

    private ModelVisual3D? _axis = null;

    private HelixViewport3D? _port = null;

    static SimpleScatterViewer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleScatterViewer),
            new FrameworkPropertyMetadata(typeof(SimpleScatterViewer)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _bound = GetTemplateChild(Bound) as BoundViewer;
        _scatter = GetTemplateChild(Scatter) as ModelVisual3D;
        _axis = GetTemplateChild(Axis) as ModelVisual3D;
        _port = GetTemplateChild(Port) as HelixViewport3D;

        SetAttachBinding();
    }

    /// <summary>
    /// true: error
    /// </summary>
    /// <returns></returns>
    private bool ValidChild()
        => _bound == null || _scatter == null || _axis == null || _port == null;

    /// <summary>
    /// true: error
    /// </summary>
    /// <returns></returns>
    private bool StrictValid()
        => ValidChild() || (Points == null || Values == null) || (Points.Length != Values.Length);

    private void SetAttachBinding()
    {
        if (ValidChild())
            throw new Exception("[SimpleScatterViewer] Current child is null.");

        BindingOperations.SetBinding(_bound!, BoundViewer.BoundSizeProperty, new Binding()
        {
            Source = this,
            Path = new PropertyPath(BoundAttach.LocationProperty)
        });

        BindingOperations.SetBinding(_bound!, BoundViewer.BoundDiameterProperty, new Binding()
        {
            Source = this,
            Path = new PropertyPath(BoundAttach.DiameterProperty)
        });
    }

    // note: 数据的值必须默认归一化过
    private Model3D CreateScatterModel()
    {

#if TestCode

        var testModel = new Model3DGroup();
        var testBuilder = new MeshBuilder(true, true, true);

        var a1 = 0.1;
        var a2 = 0.5;

        testBuilder.AddBox(new Point3D(0, 0, 0), 10, 10, 10);
        testBuilder.AddBox(new Point3D(0, 0, 0), 5, 5, 5);

        var testCount = 0;
        for (var j = testCount; j < testBuilder.TextureCoordinates.Count; ++j)
            testBuilder.TextureCoordinates[j] = new Point(a1, a1);
        testCount = testBuilder.TextureCoordinates.Count;

        for (var j = testCount; j < testBuilder.TextureCoordinates.Count; ++j)
            testBuilder.TextureCoordinates[j] = new Point(a2, a2);
        testCount = testBuilder.TextureCoordinates.Count;

        testBuilder.AddEllipsoid(Points![0], 0, 0, 0);
        for (var j = testCount; j < testBuilder.TextureCoordinates.Count; ++j)
            testBuilder.TextureCoordinates[j] = new Point(0, 0);

        testCount = testBuilder.TextureCoordinates.Count;
        testBuilder.AddEllipsoid(Points[0], 0, 0, 0);
        for (var j = testCount; j < testBuilder.TextureCoordinates.Count; ++j)
            testBuilder.TextureCoordinates[j] = new Point(1, 1);

        var testBrush = Colormaps.Converter(Colormap.Green, 0.5);

        var testScatter = new GeometryModel3D(testBuilder.ToMesh(true),
            MaterialHelper.CreateMaterial(testBrush, 0, 0, 255));

        testScatter.BackMaterial = testScatter.Material;
        //scatter.BackMaterial = MaterialHelper.CreateMaterial(Colors.White, 0.1);

        testModel.Children.Add(testScatter);
        return testModel;
#endif


        var model = new Model3DGroup();

        if (StrictValid()) return model;

        var builder = new MeshBuilder(true, true, true);
        var count = 0;

        // debug
        var percent = 0;

        for (var i = 0; i < Points!.Length; ++i)
        {
            //if (IsBox) builder.AddBox(Points[i], 2, 2, 2,
            //    BoxFaces.NegativeX | BoxFaces.NegativeZ | BoxFaces.NegativeY | BoxFaces.PositiveY | BoxFaces.PositiveX | BoxFaces.PositiveZ);
            //else builder.AddEllipsoid(Points[i], 1, 1, 1);

            var size = 0.5;
            builder.AddBox(Points[i], size, size, size, BoxFaces.All);
            //builder.AddSphere(Points[i], 0.01, 4, 4);

            var u = Values![i];

            var newTcCount = builder.TextureCoordinates.Count;
            for (var j = count; j < newTcCount; ++j)
                builder.TextureCoordinates[j] = new Point(u, u);

            count = newTcCount;

#if DEBUG
            //Debug.WriteLine($"From ScatterViewer: {i}");
            if (Points.Length > 10)
                if (i % (Points.Length / 10) == 0)
                    Debug.WriteLine($"From ScatterViewer: {(percent++) * 10}%");
#endif
        }

        // note: 这里添加这两个点纯粹是因为颜色映射居然会自己去normalize，helix这部分代码简直离谱
        // 添加一个下线限用的点
        builder.AddEllipsoid(Points[0], 0, 0, 0);
        for (var j = count; j < builder.TextureCoordinates.Count; ++j)
            builder.TextureCoordinates[j] = new Point(0, 0);

        // 添加一个上线限用的点
        count = builder.TextureCoordinates.Count;
        builder.AddEllipsoid(Points[0], 0, 0, 0);
        for (var j = count; j < builder.TextureCoordinates.Count; ++j)
            builder.TextureCoordinates[j] = new Point(1, 1);

        count = builder.TextureCoordinates.Count;
        builder.AddBox(new Point3D(0, 0, 0), 10, 10, 10);
        for (var j = count; j < builder.TextureCoordinates.Count; ++j)
            builder.TextureCoordinates[j] = new Point(0.1, 0.1);

        count = builder.TextureCoordinates.Count;
        builder.AddBox(new Point3D(5, 5, 5), 1, 1, 1);
        for (var j = count; j < builder.TextureCoordinates.Count; ++j)
            builder.TextureCoordinates[j] = new Point(0.2, 0.2);



        var brush = Colormaps.Converter(Colormap, 0.1);

        var scatter = new GeometryModel3D(builder.ToMesh(true),
            MaterialHelper.CreateMaterial(brush, 0, 1, 255));
        //MaterialHelper.CreateMaterial(brush, 100, 255, 255));

        scatter.BackMaterial = scatter.Material;
        //scatter.BackMaterial = MaterialHelper.CreateMaterial(Colors.White, 0.1);

        model.Children.Add(scatter);
        return model;
    }


    private static void OnScatterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not SimpleScatterViewer viewer) return;
        if (viewer.StrictValid()) return;

        // todo: refresh boundary

        var scatter = viewer._scatter!;
        scatter.Content = viewer.CreateScatterModel();
    }

    public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
        nameof(Points), typeof(Point3D[]), typeof(SimpleScatterViewer), new FrameworkPropertyMetadata(default(Point3D[]),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScatterChanged));

    public Point3D[]? Points
    {
        get => (Point3D[]?) GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
        nameof(Values), typeof(double[]), typeof(SimpleScatterViewer), new FrameworkPropertyMetadata(default(double[]),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnScatterChanged));

    public double[]? Values
    {
        get => (double[]?) GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    // note: 显示的小点是Box还是Sphere  这个的主要作用是节省性能资源
    public static readonly DependencyProperty IsBoxProperty = DependencyProperty.Register(
        nameof(IsBox), typeof(bool), typeof(SimpleScatterViewer), new PropertyMetadata(false));

    public bool IsBox
    {
        get => (bool) GetValue(IsBoxProperty);
        set => SetValue(IsBoxProperty, value);
    }

    public static readonly DependencyProperty ColormapProperty = DependencyProperty.Register(
        nameof(Colormap), typeof(Colormap), typeof(SimpleScatterViewer), new PropertyMetadata(Core.Colormap.Rainbow));

    public Colormap Colormap
    {
        get => (Colormap) GetValue(ColormapProperty);
        set => SetValue(ColormapProperty, value);
    }

}
