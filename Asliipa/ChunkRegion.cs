using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Whizz
{
    public class ChunkRegion(Vector2 coordinates, Chunk[,,] chunks)
    {
        /// <summary>
        /// Size of the region in chunks (cube side length)
        /// </summary>
        public const int RegionSize = 16;

        public Vector2 Coordinate { get; private set; } = coordinates;
        public Chunk[,,] Chunks  { get; private set; } = chunks;

        public void SaveOrUpdate(World world)
        {
            // does nothing if directories already exist
            Directory.CreateDirectory("./Worlds");
            Directory.CreateDirectory($"./Worlds/{world.Name}");

            // create clean chunk data for comparison
            // (differences should be stored as they're created,
            // so it does not require regenerating a whole region
            // just to save it, but oh well)

            Chunk[,,] reference = new Chunk[RegionSize, RegionSize, RegionSize];
            for (int x = 0; x < RegionSize; x++)
                for (int y = 0; y < RegionSize; y++)
                    for (int z = 0; z < RegionSize; z++)
                        reference[x, y, z] = Chunk.GenerateNewChunk(new(world.Seed), new(x, y, z));
        }
    }
}
