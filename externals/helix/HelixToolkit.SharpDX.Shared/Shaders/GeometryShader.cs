﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

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
    namespace Shaders
    {
        using Render;
        /// <summary>
        /// 
        /// </summary>
        public sealed class GeometryShader : ShaderBase
        {
            private global::SharpDX.Direct3D11.GeometryShader shader;
            internal global::SharpDX.Direct3D11.GeometryShader Shader => shader;
            public static readonly GeometryShader NullGeometryShader = new GeometryShader("NULL");
            public static readonly GeometryShaderType Type;
            /// <summary>
            /// Initializes a new instance of the <see cref="GeometryShader"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="name">The name.</param>
            /// <param name="byteCode">The byte code.</param>
            public GeometryShader(Device device, string name, byte[] byteCode) : base(name, ShaderStage.Geometry)
            {
                shader = new global::SharpDX.Direct3D11.GeometryShader(device, byteCode);
                shader.DebugName = name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GeometryShader"/> class. This is used for stream out geometry shader
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="name">The name.</param>
            /// <param name="byteCode">The byte code.</param>
            /// <param name="streamOutputElements">The stream output elements.</param>
            /// <param name="bufferStrides">The buffer strides.</param>
            /// <param name="rasterizedStream">The rasterized stream.</param>
            public GeometryShader(Device device, string name, byte[] byteCode, StreamOutputElement[] streamOutputElements, int[] bufferStrides,
                int rasterizedStream = global::SharpDX.Direct3D11.GeometryShader.StreamOutputNoRasterizedStream)
                : base(name, ShaderStage.Geometry)
            {
                shader = new global::SharpDX.Direct3D11.GeometryShader(device, byteCode, streamOutputElements, bufferStrides, rasterizedStream);
                shader.DebugName = name;
            }

            private GeometryShader(string name)
                : base(name, ShaderStage.Geometry, true)
            {

            }

            /// <summary>
            /// Binds shader to pipeline
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="bindConstantBuffer"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Bind(DeviceContextProxy context, bool bindConstantBuffer = true)
            {
                context.SetShader(this);
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                RemoveAndDispose(ref shader);
                base.OnDispose(disposeManagedResources);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator GeometryShaderType(GeometryShader s)
            {
                return Type;
            }
        }
    }
}
