using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Whizz
{
    /// <summary>
    /// A struct representing a material of something physical (primarily, tiles)
    /// </summary>
    public struct Material
    {
        public Vector2 AtlasTextureCoordinates;
        public Color TextureModulation;
    }
}
