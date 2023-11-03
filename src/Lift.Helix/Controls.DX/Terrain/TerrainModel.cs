using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;

namespace Lift.Helix.Controls.DX.Terrain;

public class TerrainModel : MeshGeometryModel3D
{
    public static readonly DependencyProperty HeightScaleProperty = DependencyProperty.Register("HeightScale", typeof(double),
        typeof(MeshGeometryModel3D),
        new PropertyMetadata(5.0, (d, e) =>
        {
            ((d as Element3D).SceneNode as TerrainMeshNode).HeightScale = (float) (double) e.NewValue;
        }));

    public double HeightScale
    {
        set
        {
            SetValue(HeightScaleProperty, value);
        }
        get
        {
            return (double) GetValue(HeightScaleProperty);
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new TerrainMeshNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);
        (core as TerrainMeshNode).HeightScale = (float) HeightScale;
    }
}
