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
        public const int TileSize = 16;
        public readonly static Vector2 TileDimensions = new(TileSize, TileSize);

        public Material Material;

        public void RenderAt(nint renderer, Vector2 screenCoord)
        {
            SDL.SDL_Rect sourceRect = new()
            {
                x = (int)Material.AtlasTextureCoordinates.X,
                y = (int)Material.AtlasTextureCoordinates.Y,
                w = TileSize,
                h = TileSize
            };

            SDL.SDL_Rect destRect = new()
            {
                x = (int)screenCoord.X,
                y = (int)screenCoord.Y,
                w = TileSize,
                h = TileSize
            };

            var res = SDL.SDL_RenderCopy(renderer, Material.AtlasTexture, ref sourceRect, ref destRect);
            if (res != 0)
                Game.VisualLoggerAgent.Log("Failed drawing texture", LogLevel.Error);
        }
    }
}
