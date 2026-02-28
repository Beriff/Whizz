using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Whizz
{
    public class ChunkRegion(Vector3 coordinates, Chunk[,,] chunks)
    {
        /// <summary>
        /// Size of the region in chunks (cube side length)
        /// </summary>
        public const int RegionSize = 16;

        public Vector3 Coordinate { get; private set; } = coordinates;
        public Chunk[,,] Chunks  { get; private set; } = chunks;

        public void SaveOrUpdate(World world)
        {
            Game.StorageLoggerAgent.Log($"Began saving region {Coordinate}", LogLevel.Debug);

            // does nothing if directories already exist
            Directory.CreateDirectory("./Worlds");
            Directory.CreateDirectory($"./Worlds/{world.Name}");

            // create clean chunk data for comparison
            // (differences should be stored as they're created,
            // so it does not require regenerating a whole region
            // just to save it, but it is not implemented yet)

            Chunk[,,] reference = new Chunk[RegionSize, RegionSize, RegionSize];
            Parallel.For(0, RegionSize, x =>
            {
                for (int y = 0; y < RegionSize; y++)
                    for (int z = 0; z < RegionSize; z++)
                        reference[x, y, z] = Chunk.GenerateNewChunk(world, new(x, y, z));
            });
                

            // compare against present chunks
            // and construct a map of tile differences

            Dictionary<(Vector3 pos, Vector3 chunkPos), Tile> tileDiffs = [];
            Parallel.For(0, RegionSize, x =>
            {
                for (int y = 0; y < RegionSize; y++)
                    for (int z = 0; z < RegionSize; z++)
                    {
                        var currentChunk = Chunks[x, y, z];
                        var currentRef = reference[x, y, z];
                        for (int tx = 0; tx < Chunk.ChunkTileSize; tx++)
                            for (int ty = 0; ty < Chunk.ChunkTileSize; ty++)
                                for (int tz = 0; tz < Chunk.ChunkTileSize; tz++)
                                    if (currentChunk[tx, ty, tz] != currentRef[tx, ty, tz])
                                        tileDiffs[(new Vector3(tx, ty, tz), new Vector3(x, y, z))] = currentChunk[tx, ty, tz];
                    }
            });
               

            using var regionStream = File.Open($"./Worlds/{world.Name}/region_{Coordinate.X}_{Coordinate.Y}_{coordinates.Z}.dat", 
                FileMode.Create, // overwrite the region file if exists
                FileAccess.Write);
            using var regionWriter = new BinaryWriter(regionStream);

            foreach(var ((pos, chunkPos), tile) in tileDiffs)
            {
                regionWriter.Write(chunkPos.X);
                regionWriter.Write(chunkPos.Y);
                regionWriter.Write(chunkPos.Z);
                regionWriter.Write(pos.X);
                regionWriter.Write(pos.Y);
                regionWriter.Write(pos.Z);
                tile.Serialize(regionStream);
            }

            Game.StorageLoggerAgent.Log($"Finished saving region {Coordinate}", LogLevel.Debug);
        }

        public static ChunkRegion Load(World world, Vector3 coordinates)
        {
            Game.StorageLoggerAgent.Log($"Began loading region {coordinates}", LogLevel.Debug);

            // fallback logic on file failure
            FileStream regionStream;
            try
            {
                regionStream = File.Open($"./Worlds/{world.Name}/region_{coordinates.X}_{coordinates.Y}_{coordinates.Z}.dat",
                FileMode.Open,
                FileAccess.Read);
            } catch (Exception e)
            {
                if (e is FileNotFoundException)
                    Game.StorageLoggerAgent.Log(
                        $"Region file region_{coordinates.X}_{coordinates.Y}_{coordinates.Z}.dat not found",
                        LogLevel.Fatal);
                else if (e is DirectoryNotFoundException)
                    Game.StorageLoggerAgent.Log(
                        $"World \"{world.Name}\" not found",
                        LogLevel.Error);
                else
                    Game.StorageLoggerAgent.Log(
                    $"Uknown error when loading region_{coordinates.X}_{coordinates.Y}_{coordinates.Z}.dat",
                    LogLevel.Error);

                return GenerateDefaultRegion();
            }

            Chunk[,,] chunks = new Chunk[RegionSize, RegionSize, RegionSize];
            using var regionReader = new BinaryReader(regionStream);

            Dictionary<(Vector3 pos, Vector3 chunkPos), Tile> tileDiffs = [];
            try
            {
                while (regionReader.BaseStream.Position < regionReader.BaseStream.Length)
                {
                    Vector3 chunkPos = new(
                        regionReader.ReadSingle(),
                        regionReader.ReadSingle(),
                        regionReader.ReadSingle());
                    Vector3 pos = new(
                        regionReader.ReadSingle(),
                        regionReader.ReadSingle(),
                        regionReader.ReadSingle());

                    Tile t = new(); t.Deserialize(regionReader);
                    tileDiffs[(pos, chunkPos)] = t;
                }
            } catch (EndOfStreamException)
            {
                Game.StorageLoggerAgent.Log(
                    $"Malformed region_{coordinates.X}_{coordinates.Y}_{coordinates.Z}.dat",
                    LogLevel.Error);
                return ChunkRegion.GenerateDefaultRegion();
            }

            // generate fresh region
            for (int x = 0; x < RegionSize; x++)
                for (int y = 0; y < RegionSize; y++)
                    for (int z = 0; z < RegionSize; z++)
                    {
                        var globalChunkOffset = new Vector3(x, y, z) + coordinates * RegionSize;
                        chunks[x, y, z] = Chunk.GenerateNewChunk(world, globalChunkOffset);
                    }
                        

            foreach(var ((pos, chunkPos), tile) in tileDiffs)
            {
                chunks[(int)chunkPos.X, (int)chunkPos.Y, (int)chunkPos.Z][pos] = tile;
            }

            regionStream.Close();
            Game.StorageLoggerAgent.Log($"Finished loading region {coordinates}", LogLevel.Debug);
            return new(coordinates, chunks);
        }

        /// <summary>
        /// Generates a region, starting at (0,0,0). Should not be used outside debug purposes,
        /// as chunks must be filled by the world rather than generated by the region itself
        /// </summary>
        public static ChunkRegion GenerateDefaultRegion()
        {
            Chunk[,,] chunks = new Chunk[RegionSize, RegionSize, RegionSize];
            Parallel.For(0, RegionSize, x =>
            {
                for (int y = 0; y < RegionSize; y++)
                    for (int z = 0; z < RegionSize; z++)
                        chunks[x, y, z] = Chunk.GenerateNewChunk(new World(WorldGenSettings.Default), new(x, y, z));
            });
                
            
            return new(Vector3.Zero, chunks);
        }
    }
}
