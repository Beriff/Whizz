using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Whizz
{
    public record struct WorldGenSettings
    {
        public int SeaLevel;
        public int Seed;
        public int HeightScale;

        public static WorldGenSettings Default => new() 
        { 
            SeaLevel = Chunk.ChunkTileSize,
            HeightScale = 255,
            Seed = 0
        };
    }

    public class World(WorldGenSettings settings)
    {
        public Vector2 Camera;
        public readonly WorldGenSettings GenSettings = settings;
        public string Name { get; set; } = "DefaultWorld";

        // Side of a cube that is loaded around the player
        protected static int ChunkLoadRadius = 3;

        // Uses chunk coordinates
        protected Dictionary<Vector2, Chunk> LoadedChunks;

        public void LoadChunksAtPoint(Vector2 worldCoordinates)
        {

        }
    }
}
