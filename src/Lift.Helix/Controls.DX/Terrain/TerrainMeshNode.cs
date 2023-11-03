using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;

namespace Lift.Helix.Controls.DX.Terrain;

public class TerrainMeshNode : MeshNode
{
    public float HeightScale
    {
        set
        {
            (RenderCore as TerrainMeshRenderCore).DataHeightScale = value;
        }
        get
        {
            return (RenderCore as TerrainMeshRenderCore).DataHeightScale;
        }
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new TerrainMeshRenderCore();
    }

    protected override IRenderTechnique OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[TerrainEffectsManager.DataSampling];
    }
}
