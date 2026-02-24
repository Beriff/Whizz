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
        public static nint AtlasTexture;

        public string Name;
        public Vector2 AtlasTextureCoordinates;
        public Color TextureModulation;

        public readonly bool IsOpaque => TextureModulation.A == 255;

        public Material(string name, Vector2 textureIndex, Color? modulation = null)
        {
            Name = name;
            AtlasTextureCoordinates = textureIndex * Tile.TileSize;
            TextureModulation = modulation ?? Color.White;
        }
    }
}
