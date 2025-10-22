﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UO3D.Runtime.RHI.Resources;

public interface IRenderSwapChain
{
    public TextureFormat BackbufferFormat { get; }
    public RenderTarget? Acquire(IRenderContext context);

}
