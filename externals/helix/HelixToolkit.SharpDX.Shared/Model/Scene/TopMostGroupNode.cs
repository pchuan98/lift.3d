﻿/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;

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
    namespace Model.Scene
    {
        using Core;
        using Cameras;
        /// <summary>
        /// Provides a way to render child elements always on top of other elements.
        /// This is rendered at the same level of screen spaced group items.
        /// Child items do not support post effects.
        /// </summary>
        public class TopMostGroupNode : GroupNode
        {
            private bool enableTopMost = true;
            public bool EnableTopMost
            {
                set
                {
                    if (SetAffectsRender(ref enableTopMost, value))
                    {
                        RenderType = value ? RenderType.ScreenSpaced : RenderType.Opaque;
                    }
                }
                get => enableTopMost;
            }

            public TopMostGroupNode()
            {
                AffectsGlobalVariable = true;
            }

            protected override RenderCore OnCreateRenderCore()
            {
                var core = new TopMostMeshRenderCore();
                return core;
            }
        }
    }
}
