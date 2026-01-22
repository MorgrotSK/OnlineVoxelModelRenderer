using LibNoise.Filter;
using LibNoise.Primitive;
using System.Numerics;
using FE3.VoxelRenderer.Utils.Octree;
using SharedClass.World;

namespace CastleServer.Chunks
{
    internal class CaveGenerator
    {
        private const float Threshold = 0.005f;
        private readonly short _floorLevel;


        private const int SampleRate = 2;
        private const byte NeededTrues = 1;

        private readonly int _sampleWidth;
        private readonly int _sampleHeight;


        private RidgedMultiFractal _fractalNoise1;
        private RidgedMultiFractal _fractalNoise2;
        private RidgedMultiFractal _fractalNoise3;

        private Vector3[] offsets;

        public CaveGenerator(short floorLevel)
        {
            this._floorLevel = floorLevel;
            this._sampleHeight = floorLevel + 5;
            this._sampleWidth = ChunkProperty.Width / (SampleRate * 2 / 3);

            this.InitializeFractalNoises();

            this.offsets =
            [
                new Vector3(0, 0, 0), // The base point itself
                new Vector3(SampleRate, 0, 0),
                new Vector3(0, SampleRate, 0),
                new Vector3(0, 0, SampleRate),
                new Vector3(SampleRate, SampleRate, 0),
                new Vector3(0, SampleRate, SampleRate),
                new Vector3(SampleRate, 0, SampleRate),
                new Vector3(SampleRate, SampleRate, SampleRate)
            ];

        }

        public void GenerateCaves(FlatOctree chunk, Vector2 cornerPosition)
        {

            bool[,,] samples = new bool[this._sampleWidth, this._sampleHeight, this._sampleWidth];

            Parallel.For(0, (ChunkProperty.Width / SampleRate) + 1, x =>
            {
                for (int z = 0; z < ChunkProperty.Width; z += SampleRate)
                {
                    for (int y = 0; y < _floorLevel + 5; y += SampleRate)
                    {
                        int globalX = (int)cornerPosition.X + x * SampleRate;
                        int globalZ = (int)cornerPosition.Y + z;
                        int globalY = y;

                        // Generate a base noise
                        float sampleValue = GenerateComplexCaveNoiseValueFloat(globalX, globalY, globalZ);
                        try
                        {
                            samples[x * SampleRate, y, z] = sampleValue <= Threshold;
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(samples.GetLength(0) + " " + x * SampleRate + " " + samples.GetLength(1) + " " + y + " " + samples.GetLength(2) + " " + z);

                        }


                    }
                }
            });

            Parallel.For((int)0, ChunkProperty.Width, x =>
            {
                for (int z = 0; z < ChunkProperty.Width; z++)
                {
                    for (int y = 0; y < _floorLevel + 5; y++) // Loop through every block, not just sampled points
                    {
                        int globalX = (int)cornerPosition.X + x;
                        int globalZ = (int)cornerPosition.Y + z;
                        int globalY = y;
                        Vector3 dumpPoint = new Vector3(x, y, z);

                        int trueCount = CheckNearestSamples(dumpPoint, ref samples);

                        if (trueCount >= NeededTrues)
                        {
                            chunk.Insert(globalX, globalY, globalZ, 0);
                            for (int i = 1; i < SampleRate; i++)
                            {
                                chunk.Insert(globalX, globalY + i, globalZ, 0);
                            }
                        }
                        y += SampleRate - 1;

                    }
                }
            });

        }

        public int CheckNearestSamples(Vector3 point, ref bool[,,] samples)
        {
            // Adjust the point to the nearest grid intersection
            int ix = (int)Math.Floor(point.X / SampleRate) * SampleRate;
            int iy = (int)Math.Floor(point.Y / SampleRate) * SampleRate;
            int iz = (int)Math.Floor(point.Z / SampleRate) * SampleRate;

            int trueCount = 0;

            foreach (Vector3 offset in offsets)
            {
                int neighborX = ix + (int)offset.X / SampleRate;
                int neighborY = iy + (int)offset.Y / SampleRate;
                int neighborZ = iz + (int)offset.Z / SampleRate;

                // Ensure the neighbor indices are within the bounds of the samples array
                if (neighborX >= 0 && neighborX < this._sampleWidth && neighborY >= 0 && neighborY < this._sampleHeight && neighborZ >= 0 && neighborZ < this._sampleWidth)
                {
                    if (samples[neighborX, neighborY, neighborZ])
                    {
                        trueCount++;

                        if (trueCount == NeededTrues)
                        {
                            // Return the count immediately upon finding the required number of true samples
                            return trueCount;
                        }
                    }
                }
            }

            // Return the count of true samples found, which may be less than 2
            return trueCount;
        }

        private float GenerateComplexCaveNoiseValueFloat(float x, float y, float z)
        {

            double noiseValue = this._fractalNoise1.GetValue(x, y, z) * this._fractalNoise2.GetValue(x, y, z) * this._fractalNoise3.GetValue(x, y, z);

            // Return the thresholded value, inverting the result
            return (float)Math.Abs(noiseValue);
        }

        private void InitializeFractalNoises()
        {
            _fractalNoise1 = new RidgedMultiFractal()
            {
                OctaveCount = 1,
                Frequency = 0.013f,
                SpectralExponent = 0.4f,
                Primitive3D = new SimplexPerlin(2001, LibNoise.NoiseQuality.Fast)
            };

            _fractalNoise2 = new RidgedMultiFractal()
            {
                OctaveCount = 1,
                Frequency = 0.013f,
                SpectralExponent = 0.4f,
                Primitive3D = new SimplexPerlin(2002, LibNoise.NoiseQuality.Fast)
            };

            _fractalNoise3 = new RidgedMultiFractal()
            {
                OctaveCount = 1,
                Frequency = 0.013f,
                SpectralExponent = 0.4f,
                Primitive3D = new SimplexPerlin(2003, LibNoise.NoiseQuality.Fast)
            };
        }


    }
}
