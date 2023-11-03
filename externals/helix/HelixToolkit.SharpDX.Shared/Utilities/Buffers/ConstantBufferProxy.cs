﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Runtime.CompilerServices;
using SDX11 = SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Threading;
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
    namespace Utilities
    {
        using Shaders;
        using Render;
        using System.Diagnostics;

        /// <summary>
        ///
        /// </summary>
        public sealed class ConstantBufferProxy : BufferProxyBase
        {
            /// <summary>
            ///
            /// </summary>
            public bool Initialized
            {
                get
                {
                    return buffer != null;
                }
            }

            internal BufferDescription bufferDesc;

            public string Name
            {
                private set; get;
            }

            private readonly object lockObj = new object();

            internal Dictionary<string, ConstantBufferVariable> VariableDictionary { get; } = new Dictionary<string, ConstantBufferVariable>();
            /// <summary>
            ///
            /// </summary>
            /// <param name="name"></param>
            /// <param name="structSize"></param>
            /// <param name="bindFlags"></param>
            /// <param name="cpuAccessFlags"></param>
            /// <param name="optionFlags"></param>
            /// <param name="usage"></param>
            /// <param name="strideSize"></param>
            public ConstantBufferProxy(string name, int structSize, BindFlags bindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None,
                ResourceUsage usage = ResourceUsage.Default, int strideSize = 0)
                : base(structSize, bindFlags)
            {
                if (structSize % 16 != 0)
                {
                    throw new ArgumentException("Constant buffer struct size must be multiple of 16 bytes");
                }
                Name = name;
                bufferDesc = new BufferDescription()
                {
                    SizeInBytes = structSize,
                    BindFlags = bindFlags,
                    CpuAccessFlags = cpuAccessFlags,
                    OptionFlags = optionFlags,
                    Usage = usage,
                    StructureByteStride = strideSize
                };
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="description"></param>
            public ConstantBufferProxy(ConstantBufferDescription description)
                : base(description.StructSize, description.BindFlags)
            {
                if (description.StructSize % 16 != 0)
                {
                    throw new ArgumentException("Constant buffer struct size must be multiple of 16 bytes");
                }
                Name = description.Name;
                bufferDesc = new BufferDescription()
                {
                    SizeInBytes = description.StructSize,
                    BindFlags = description.BindFlags,
                    CpuAccessFlags = description.CpuAccessFlags,
                    OptionFlags = description.OptionFlags,
                    Usage = description.Usage,
                    StructureByteStride = description.StrideSize
                };
                foreach (var var in description.Variables)
                {
                    AddVariable(var);
                }
            }

            public void AddVariable(ConstantBufferVariable var)
            {
                if (!VariableDictionary.TryGetValue(var.Name, out var v))
                {
                    VariableDictionary.Add(var.Name, var);
                }
                else if (v.StartOffset != var.StartOffset || v.Size != var.Size)
                {
                    throw new ArgumentException($"Variable {var.Name} already exists in constant buffer definition. " +
                                                $"But start offset {var.StartOffset} and {v.StartOffset} or sizes {var.Size} and {v.Size} are not match");
                }
            }

            /// <summary>
            /// <see cref="ConstantBufferProxy.CreateBuffer(Device)"/>
            /// </summary>
            /// <param name="device"></param>
            public void CreateBuffer(Device device)
            {
                lock (lockObj)
                {
                    RemoveAndDispose(ref buffer);
                    buffer = new SDX11.Buffer(device, bufferDesc);                
                }
            }

            /// <summary>
            /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, ref T)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadDataToBuffer<T>(DeviceContextProxy context, ref T data) where T : unmanaged
            {
                lock (lockObj)
                {
                    if (bufferDesc.Usage == ResourceUsage.Dynamic)
                    {     
                        Debug.Assert(buffer.Description.SizeInBytes >= UnsafeHelper.SizeOf<T>());            
                        var dataBox = context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None);                           
                        UnsafeHelper.Write(dataBox.DataPointer, ref data);
                        context.UnmapSubresource(buffer, 0);
                    }
                    else
                    {
                        context.UpdateSubresource(ref data, buffer);
                    }                
                }
            }

            /// <summary>
            /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, T[], int)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadDataToBuffer<T>(DeviceContextProxy context, T[] data, int count) where T : unmanaged
            {
                UploadDataToBuffer(context, data, count, 0);
            }

            /// <summary>
            /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, T[], int, int)"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="context"></param>
            /// <param name="data"></param>
            /// <param name="count"></param>
            /// <param name="offset"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadDataToBuffer<T>(DeviceContextProxy context, T[] data, int count, int offset) where T : unmanaged
            {
                lock (lockObj)
                {
                    if (bufferDesc.Usage == ResourceUsage.Dynamic)
                    {
                        Debug.Assert(count * UnsafeHelper.SizeOf<T>() <= buffer.Description.SizeInBytes);
                        var dataBox = context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None);                       
                        UnsafeHelper.Write(dataBox.DataPointer, data, offset, count);
                        context.UnmapSubresource(buffer, 0);
                    }
                    else
                    {
                        context.UpdateSubresource(data, buffer);
                    }                
                }
            }

            /// <summary>
            /// <see cref="ConstantBufferProxy.UploadDataToBuffer(DeviceContextProxy, Action{DataBox})"/>
            /// </summary>
            /// <param name="context"></param>
            /// <param name="writeFuc"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UploadDataToBuffer(DeviceContextProxy context, Action<DataBox> writeFuc)
            {
                lock (lockObj)
                {
                    if (bufferDesc.Usage == ResourceUsage.Dynamic)
                    {
                        var dataBox = context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None);
                        writeFuc?.Invoke(dataBox);
                        context.UnmapSubresource(buffer, 0);
                    }
                    else
                    {
    #if DEBUG
                        throw new Exception("Constant buffer must be dynamic to use this function.");
    #endif
                    }                
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataBox Map(DeviceContextProxy context)
            {
                Monitor.Enter(lockObj);
                return context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DataStream MapToStream(DeviceContextProxy context)
            {
                Monitor.Enter(lockObj);
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out var stream);
                return stream;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Unmap(DeviceContextProxy context)
            {
                context.UnmapSubresource(buffer, 0);
                Monitor.Exit(lockObj);
            }

            /// <summary>
            /// Special function to recreate existing constant buffer to new size.
            /// </summary>
            /// <param name="device"></param>
            /// <param name="structSize"></param>
            public void ResizeBuffer(Device device, int structSize)
            {
                if (structSize % 16 != 0)
                {
                    throw new ArgumentException("Constant buffer struct size must be multiple of 16 bytes");
                }
                lock (lockObj)
                {
                    RemoveAndDispose(ref buffer);
                    bufferDesc.SizeInBytes = structSize;
                    buffer = new SDX11.Buffer(device, bufferDesc);                
                }
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                RemoveAndDispose(ref buffer);
                base.OnDispose(disposeManagedResources);
            }
            /// <summary>
            /// Performs an implicit conversion from <see cref="ConstantBufferProxy"/> to <see cref="SDX11.Buffer"/>.
            /// </summary>
            /// <param name="proxy">The proxy.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator SDX11.Buffer(ConstantBufferProxy proxy)
            {
                return proxy?.buffer;
            }
            /// <summary>
            /// Gets the <see cref="ConstantBufferVariable"/> with the specified name.
            /// </summary>
            /// <value>
            /// The <see cref="ConstantBufferVariable"/>.
            /// </value>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public ConstantBufferVariable this[string name]
            {
                get => VariableDictionary[name];
            }
            /// <summary>
            /// Tries the name of the get variable by.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="variable">The variable.</param>
            /// <returns></returns>
            public bool TryGetVariableByName(string name, out ConstantBufferVariable variable)
            {
                return VariableDictionary.TryGetValue(name, out variable);
            }
        }
    }
}