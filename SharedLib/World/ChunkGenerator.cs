using System.Numerics;
using FE3.VoxelRenderer.Utils.Octree;
using SharedClass.World;
using SimplexNoise;

namespace SharedClass;

public class ChunkGenerator
{
        private const short FloorLevel = 1;

        private const byte TopBlock = 1;
        private const byte SecondBlock = 1;
        private const byte MainBlock = 1;

        // Parameters for Simplex noise
        private const float Scale = 0.5f;
        private const float HeightMultiplier = 0.12f;

        public ChunkGenerator()
        {
            Noise.Seed = 2001;
        }

        public FlatOctree GenerateChunk(Vector2 position)
        {
            Vector2 cornerPosition = new Vector2(
                position.X - ChunkProperty.Width / 2,
                position.Y - ChunkProperty.Width / 2
            );
            
            FlatOctree chunk = new FlatOctree(depth: 6);

            for (int x = 0; x < ChunkProperty.Width; ++x)
            {
                for (int z = 0; z < ChunkProperty.Width; ++z)
                {
                    // Convert local coordinates to global coordinates
                    int globalX = (int)cornerPosition.X + x;
                    int globalZ = (int)cornerPosition.Y + z;

                    int maxHeight = (int)this.GenerateHeight(globalX, globalZ) + FloorLevel;
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

            return chunk;
        }


        private float GenerateHeight(int x, int z)
        {
            // Use noise to generate height
            float noiseValue = Noise.CalcPixel2D((int)(x * Scale), (int)(z * Scale), 0.025f);

            // Normalize and scale the noise value
            return (noiseValue + 1) / 2 * HeightMultiplier;
        }
    }
   


