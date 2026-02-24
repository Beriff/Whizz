using SDL2;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Whizz
{
    /// <summary>
    /// A tile is a basic component of the world, that does not interact with it by itself,
    /// but rather is controlled by a controller class (tile is a "dumb" data).
    /// </summary>
    public struct Tile
    {
        public Material Material;

        public void DrawAt(nint renderer, Vector2 screenCoord)
        {
            
        }
    }
}
