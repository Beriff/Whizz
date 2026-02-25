using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Whizz
{
    public class Chunk : IStreamSerializable
    {
        public const int ChunkTileSize = 32;
        public readonly static Vector2 ChunkTileDimensions = new(ChunkTileSize, ChunkTileSize);

        public const int ChunkPixelSize = ChunkTileSize * Tile.TileSize;

        public Tile[,,] Grid = new Tile[ChunkTileSize, ChunkTileSize, ChunkTileSize];

        /// <summary>
        /// Renders a column of tiles belonging to this chunk. If the tile is opaque, it renders the one below it
        /// until it encounters an opaque tile or reaches the end of the chunk
        /// </summary>
        /// <returns> Whether or not the rendered column is opaque (at least one rendered tile is opaque)</returns>
        public bool RenderColumnAt(nint renderer, Vector2 tileChunkCoords, Vector2 screenCoords, int startZ)
        {

            int endZ = startZ;
            while (true)
            {
                Tile currentTile = Grid[(int)tileChunkCoords.X, (int)tileChunkCoords.Y, endZ];
                if (currentTile.Material.IsOpaque) break;
                endZ++;
                if (endZ == ChunkTileSize - 1) break;
            }

            for (int z = startZ; z <= endZ; z++)
            {
                Grid[(int)tileChunkCoords.X, (int)tileChunkCoords.Y, z].RenderAt(renderer, screenCoords);
            }

            if (endZ == ChunkTileSize - 1 &&
                !Grid[(int)tileChunkCoords.X, (int)tileChunkCoords.Y, endZ].Material.IsOpaque)
                return false;
            return true;
        }

        public void RenderChunkAt(nint renderer, Vector2 screenCoords, int localZ)
        {
            for (int x = 0; x < ChunkTileSize; x++)
            {
                for (int y = 0; y < ChunkTileSize; y++)
                {
                    Vector2 tileChunkCoord = new(x, y);
                    RenderColumnAt(renderer,
                        tileChunkCoord,
                        screenCoords + Tile.TileDimensions * tileChunkCoord,
                        localZ);
                }
            }


        }

#if DEBUG
        public Chunk DebugFill(Tile t)
        {
            for (int x = 0; x < ChunkTileSize; x++)
                for (int y = 0; y < ChunkTileSize; y++)
                    for (int z = 0; z < ChunkTileSize; z++)
                        Grid[x, y, z] = t;

            return this;
        }
#endif

        public Chunk GenerateChunk(Noise noise, Vector2 coord)
        {
            const float frequency = 0.1f;

            for (int x = 0; x < ChunkTileSize; x++)
                for (int y = 0; y < ChunkTileSize; y++)
                    for (int z = 0; z < ChunkTileSize; z++)
                    {
                        float n = noise.Fractal(
                            new Vector2((coord.X + x) * frequency, (coord.Y + y) * frequency),
                            octaves: 6,
                            lacunarity: 2.0f,
                            persistence: 0.5f
                        );

                        int ix = (int)((n * 0.5 + 0.5) * 4);
                        Material mat = new($"mat{ix}", new(ix, 0));
                        Tile t = new() { Material = mat };
                        Grid[x, y, z] = t;
                    }

            return this;
        }


        public void Serialize(Stream stream)
        {
            for (int x = 0; x < ChunkTileSize; x++)
                for (int y = 0; y < ChunkTileSize; y++)
                    for (int z = 0; z < ChunkTileSize; z++)
                        Grid[x, y, z].Serialize(stream);
        }

        public void Deserialize(Stream stream)
        {
            for (int x = 0; x < ChunkTileSize; x++)
            {
                for (int y = 0; y < ChunkTileSize; y++)
                {
                    for (int z = 0; z < ChunkTileSize; z++)
                    {
                        Tile t = new();
                        t.Deserialize(stream);
                        Grid[x, y, z] = t;
                    }
                }
            }
        }

    }
    public class World
    {
        public Vector2 Camera;

        // Side of a cube that is loaded around the player
        protected static int ChunkLoadRadius = 3;
        protected Random RNG;

        // Uses chunk coordinates
        protected Dictionary<Vector2, Chunk> LoadedChunks;

        public World(int seed)
        {
            RNG = new Random(seed);
        }

        public void LoadChunksAtPoint(Vector2 worldCoordinates)
        {

        }
    }
}
