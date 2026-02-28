using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

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
                if (currentTile.GetMaterial().IsOpaque) break;
                endZ++;
                if (endZ == ChunkTileSize - 1) break;
            }

            for (int z = startZ; z <= endZ; z++)
            {
                Grid[(int)tileChunkCoords.X, (int)tileChunkCoords.Y, z].RenderAt(renderer, screenCoords);
            }

            if (endZ == ChunkTileSize - 1 &&
                !Grid[(int)tileChunkCoords.X, (int)tileChunkCoords.Y, endZ].GetMaterial().IsOpaque)
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

        public Chunk GenerateChunk(World w, Vector3 coord)
        {
            const float frequency = 0.01f;

            Chunk c = new();
            Noise2D noise = new(w.GenSettings.Seed);
            Vector3 tileOffset = coord * Chunk.ChunkTileSize;
            // the surface is centered at Z = 16 (middle of the Z=0 chunk), instead of Z = 0 (top of Z=0 chunk)
            float heightmapOffset = Chunk.ChunkTileSize / 2f; 

            for (int x = 0; x < ChunkTileSize; x++)
                for (int y = 0; y < ChunkTileSize; y++)
                {
                    float height = noise.Fractal(new Vector2(x, y) * frequency, 6, 2f, .5f) * w.GenSettings.HeightScale + heightmapOffset;
                    for (int z = 0; z < ChunkTileSize; z++)
                    {
                        int globalZ = (int)tileOffset.Z + z;
                        if ((height - globalZ) < .1f)
                            c[x, y, z] = new() { MaterialId = 0 };
                        else if (globalZ > height)
                        {
                            c[x, y, z] = new() { MaterialId = 1 };
                        }
                            
                        else
                            c[x, y, z] = new() { MaterialId = 1 };
                    }
                }
                    


            return c;
        }

        public static Chunk GenerateNewChunk(World w, Vector3 coord) => new Chunk().GenerateChunk(w, coord);

        public void Serialize(Stream stream)
        {
            for (int x = 0; x < ChunkTileSize; x++)
                for (int y = 0; y < ChunkTileSize; y++)
                    for (int z = 0; z < ChunkTileSize; z++)
                        Grid[x, y, z].Serialize(stream);
        }

        public void Deserialize(BinaryReader reader)
        {
            for (int x = 0; x < ChunkTileSize; x++)
            {
                for (int y = 0; y < ChunkTileSize; y++)
                {
                    for (int z = 0; z < ChunkTileSize; z++)
                    {
                        Tile t = new();
                        t.Deserialize(reader);
                        Grid[x, y, z] = t;
                    }
                }
            }
        }

        public Tile this[int x, int y, int z]
        {
            get => Grid[x, y, z];
            set => Grid[x, y, z] = value;
        }
        public Tile this[Vector3 pos]
        {
            get => Grid[(int)pos.X, (int)pos.Y, (int)pos.Z];
            set => Grid[(int)pos.X, (int)pos.Y, (int)pos.Z] = value;
        }
    }
}
