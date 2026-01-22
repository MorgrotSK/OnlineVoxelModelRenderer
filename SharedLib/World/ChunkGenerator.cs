using System.Numerics;
using CastleServer.Chunks;
using FE3.VoxelRenderer.Utils.Octree;
using SharedClass.World;
using SimplexNoise;

namespace SharedClass;

public static class ChunkGenerator
{
        private const short FloorLevel = 4;

        private const byte TopBlock = 4;
        private const byte SecondBlock = 5;
        private const byte MainBlock = 3;

        // Parameters for Simplex noise
        private const float Scale = 0.02f;
        private const float HeightMultiplier = 0.022f;
        
        private static readonly CaveGenerator CaveGenerator = new(FloorLevel);

        public static void SetSeed(string seed)
        {
            Noise.Seed = seed.GetHashCode();
        }

        public static FlatOctree GenerateChunk(Vector2 position)
        {
            Vector2 cornerPosition = new Vector2(
                position.X - ChunkProperty.Width / 2,
                position.Y - ChunkProperty.Width / 2
            );
            
            FlatOctree chunk = new FlatOctree(depth: 6);
            int baseX = (int)position.X * ChunkProperty.Width;
            int baseZ = (int)position.Y * ChunkProperty.Width;

            for (int x = 0; x < ChunkProperty.Width; ++x)
            {
                for (int z = 0; z < ChunkProperty.Width; ++z)
                {
                    // Convert local coordinates to global coordinates
                    int globalX = baseX + x;
                    int globalZ = baseZ + z;

                    int maxHeight = (int)GenerateHeight(globalX, globalZ) + FloorLevel;
                    if (maxHeight > ChunkProperty.Height) maxHeight = ChunkProperty.Height - 5;

                    for (int y = 0; y < maxHeight; ++y)
                    {
                        if (y == maxHeight - 1)
                        {
                            chunk.Insert(x, y, z, TopBlock); // Grass block on top
                        }
                        else if (y >= maxHeight - 3 && y < maxHeight - 1)
                        {
                            chunk.Insert(x, y, z, SecondBlock); // Dirt block
                        }
                        else
                        {
                            chunk.Insert(x, y, z, MainBlock); // Stone block
                        }
                    }
                }
            }
            
           //CaveGenerator.GenerateCaves(chunk, cornerPosition);

            return chunk;
        }


        private static float GenerateHeight(int x, int z)
        {
            float noiseValue = Noise.CalcPixel2D(x, z, Scale);

            return noiseValue * HeightMultiplier;
        }

    }
   


