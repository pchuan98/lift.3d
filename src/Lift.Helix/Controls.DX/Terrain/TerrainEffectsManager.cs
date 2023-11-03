using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX.Shaders;
using HelixToolkit.Wpf.SharpDX;

namespace Lift.Helix.Controls.DX.Terrain;


public class TerrainEffectsManager : DefaultEffectsManager
{
    public const string DataSampling = "DataSampling";
    public const string NoiseMesh = "NoiseMesh";
    public const string TexData = "texData";
    public const string TexDataSampler = "texDataSampler";


    public static ShaderDescription VSDataSampling = new ShaderDescription(nameof(VSDataSampling), ShaderStage.Vertex,
        new ShaderReflector(), Properties.Resoruces.Terrain.vsMeshDataSampling);

    public static ShaderDescription PSDataSampling = new ShaderDescription(nameof(PSDataSampling), ShaderStage.Pixel,
        new ShaderReflector(), Properties.Resoruces.Terrain.psMeshDataSampling);

    public static ShaderDescription PSCustomPoint = new ShaderDescription(nameof(PSCustomPoint), ShaderStage.Pixel,
        new ShaderReflector(), Properties.Resoruces.Terrain.psCustomPoint);

    public TerrainEffectsManager()
    {
        LoadCustomTechniqueDescriptions();
    }

    private void LoadCustomTechniqueDescriptions()
    {
        var dataSampling = new TechniqueDescription(DataSampling)
        {
            InputLayoutDescription = new InputLayoutDescription(Properties.Resoruces.Terrain.vsMeshDataSampling, DefaultInputLayout.VSInput),
            PassDescriptions = new[]
            {
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            VSDataSampling,
                            PSDataSampling
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            VSDataSampling,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
        };
        var noiseMesh = new TechniqueDescription(NoiseMesh)
        {
            InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
            PassDescriptions = new[]
            {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
        };

        AddTechnique(dataSampling);
        AddTechnique(noiseMesh);

        var points = GetTechnique(DefaultRenderTechniqueNames.Points);
        points.AddPass(new ShaderPassDescription("CustomPointPass")
        {
            ShaderList = new[]
            {
                    DefaultVSShaderDescriptions.VSPoint,
                    DefaultGSShaderDescriptions.GSPoint,
                    PSCustomPoint
                },
            BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
            DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual

        });
    }
}
