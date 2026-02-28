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
    public struct Material : IIdentified
    {
        public static nint AtlasTexture;
        public readonly static Registry<Material> Registry = new();

        public string Name;
        public Vector2 AtlasTextureCoordinates;
        public Color TextureModulation;
        public ushort Id { get; set; }

        public readonly bool IsOpaque => TextureModulation.A == 255;

        public Material(string name, Vector2 textureIndex, Color? modulation = null)
        {
            Name = name;
            AtlasTextureCoordinates = textureIndex * Tile.TileSize;
            TextureModulation = modulation ?? Color.White;
            Id = Registry.Register(this);
        }

        static Material()
        {
            new Material("Dirt", new Vector2(1, 0));
            new Material("Stone", new Vector2(2, 0));
            new Material("Air", new Vector2(0, 1));
        }
    }
}
