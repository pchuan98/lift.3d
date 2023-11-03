﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Format = global::SharpDX.DXGI.Format;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Model;
        using Model.Scene;
        using Render;
        using Shaders;
        using Components;

        /// <summary>
        /// 
        /// </summary>
        public interface IPostEffectMeshXRay : IPostEffect
        {
            /// <summary>
            /// Gets or sets the color.
            /// </summary>
            /// <value>
            /// The color.
            /// </value>
            Color4 Color
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets the outline fading factor.
            /// </summary>
            /// <value>
            /// The outline fading factor.
            /// </value>
            float OutlineFadingFactor
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
            /// </summary>
            /// <value>
            ///   <c>true</c> if [double pass]; otherwise, <c>false</c>.
            /// </value>
            bool EnableDoublePass
            {
                set; get;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class PostEffectMeshXRayCore : RenderCore, IPostEffectMeshXRay
        {
            #region Variables
            private readonly List<KeyValuePair<SceneNode, IEffectAttributes>> currentCores = new List<KeyValuePair<SceneNode, IEffectAttributes>>();
            private readonly ConstantBufferComponent modelCB;
            private BorderEffectStruct modelStruct;
            #endregion
            #region Properties
            private string effectName = DefaultRenderTechniqueNames.PostEffectMeshXRay;
            /// <summary>
            /// Gets or sets the name of the effect.
            /// </summary>
            /// <value>
            /// The name of the effect.
            /// </value>
            public string EffectName
            {
                set
                {
                    SetAffectsCanRenderFlag(ref effectName, value);
                }
                get
                {
                    return effectName;
                }
            }

            /// <summary>
            /// Gets or sets the color of the border.
            /// </summary>
            /// <value>
            /// The color of the border.
            /// </value>
            public Color4 Color
            {
                set
                {
                    SetAffectsRender(ref modelStruct.Color, value);
                }
                get
                {
                    return modelStruct.Color;
                }
            }

            /// <summary>
            /// Outline fading
            /// </summary>
            public float OutlineFadingFactor
            {
                set
                {
                    SetAffectsRender(ref modelStruct.Param.M11, value);
                }
                get
                {
                    return modelStruct.Param.M11;
                }
            }

            private bool doublePass = false;
            /// <summary>
            /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
            /// </summary>
            /// <value>
            ///   <c>true</c> if [double pass]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableDoublePass
            {
                set
                {
                    SetAffectsRender(ref doublePass, value);
                }
                get
                {
                    return doublePass;
                }
            }
            #endregion
            /// <summary>
            /// Initializes a new instance of the <see cref="PostEffectMeshXRayCore"/> class.
            /// </summary>
            public PostEffectMeshXRayCore() : base(RenderType.PostEffect)
            {
                modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
                Color = global::SharpDX.Color.Blue;
            }


            protected override bool OnAttach(IRenderTechnique technique)
            {
                return true;
            }

            protected override void OnDetach()
            {
            }

            /// <summary>
            /// Called when [render].
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="deviceContext">The device context.</param>
            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                var buffer = context.RenderHost.RenderBuffer;
                var dPass = EnableDoublePass;
                var depthStencilBuffer = buffer.DepthStencilBufferNoMSAA;
                deviceContext.SetRenderTarget(depthStencilBuffer, buffer.FullResPPBuffer.CurrentRTV);
                var viewport = context.Viewport;
                deviceContext.SetViewport(ref viewport);
                deviceContext.SetScissorRectangle(ref viewport);
                deviceContext.ClearDepthStencilView(depthStencilBuffer, DepthStencilClearFlags.Stencil, 1, 0);
                if (dPass)
                {
                    for (var i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
                    {
                        var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                        if (mesh.TryGetPostEffect(EffectName, out var effect))
                        {
                            currentCores.Add(new KeyValuePair<SceneNode, IEffectAttributes>(mesh, effect));
                            context.CustomPassName = DefaultPassNames.EffectMeshXRayP1;
                            var pass = mesh.EffectTechnique[DefaultPassNames.EffectMeshXRayP1];
                            if (pass.IsNULL)
                            {
                                continue;
                            }
                            pass.BindShader(deviceContext);
                            pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                            mesh.RenderCustom(context, deviceContext);
                        }
                    }
                    modelCB.Upload(deviceContext, ref modelStruct);
                    for (var i = 0; i < currentCores.Count; ++i)
                    {
                        var mesh = currentCores[i];
                        var effect = mesh.Value;
                        var color = Color;
                        if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out var attribute) && attribute is string colorStr)
                        {
                            color = colorStr.ToColor4();
                        }
                        if (modelStruct.Color != color)
                        {
                            modelStruct.Color = color;
                            modelCB.Upload(deviceContext, ref modelStruct);
                        }

                        context.CustomPassName = DefaultPassNames.EffectMeshXRayP2;
                        var pass = mesh.Key.EffectTechnique[DefaultPassNames.EffectMeshXRayP2];
                        if (pass.IsNULL)
                        {
                            continue;
                        }
                        pass.BindShader(deviceContext);
                        pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                        mesh.Key.RenderCustom(context, deviceContext);
                    }
                    currentCores.Clear();
                }
                else
                {
                    modelCB.Upload(deviceContext, ref modelStruct);
                    for (var i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
                    {
                        var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                        if (mesh.TryGetPostEffect(EffectName, out var effect))
                        {
                            var color = Color;
                            if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out var attribute) && attribute is string colorStr)
                            {
                                color = colorStr.ToColor4();
                            }
                            if (modelStruct.Color != color)
                            {
                                modelStruct.Color = color;
                                modelCB.Upload(deviceContext, ref modelStruct);
                            }
                            context.CustomPassName = DefaultPassNames.EffectMeshXRayP2;
                            var pass = mesh.EffectTechnique[DefaultPassNames.EffectMeshXRayP2];
                            if (pass.IsNULL)
                            {
                                continue;
                            }
                            pass.BindShader(deviceContext);
                            pass.BindStates(deviceContext, StateType.BlendState);
                            deviceContext.SetDepthStencilState(pass.DepthStencilState, 0);
                            mesh.RenderCustom(context, deviceContext);
                        }
                    }
                }
            }

            protected override bool OnUpdateCanRenderFlag()
            {
                return IsAttached && !string.IsNullOrEmpty(EffectName);
            }
        }
    }
}
