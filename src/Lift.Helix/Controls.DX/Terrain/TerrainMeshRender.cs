using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;

namespace Lift.Helix.Controls.DX.Terrain;

public class TerrainMeshRenderCore : MeshRenderCore
{
    private float dataHeightScale = 5;
    public float DataHeightScale
    {
        set
        {
            SetAffectsRender(ref dataHeightScale, value);
        }
        get { return dataHeightScale; }
    }

    protected override void OnUpdatePerModelStruct(RenderContext context)
    {
        base.OnUpdatePerModelStruct(context);
        modelStruct.Params.Y = dataHeightScale;
    }
}
