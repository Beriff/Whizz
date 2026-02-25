using System;
using System.Numerics;

namespace Whizz
{
    public class Noise(int seed)
    {
        private readonly int Seed = seed;

        private static float Fade(float t)
            => t * t * t * (t * (t * 6 - 15) + 10);

        private static float Lerp(float a, float b, float t)
            => a + t * (b - a);

        private Vector3 Grad(int x, int y, int z)
        {
            int h =
                x * 374761393 ^
                y * 668265263 ^
                z * 2147483647 ^
                Seed * 1442695041;

            h = (h ^ (h >> 13)) * 1274126177;
            h ^= h >> 16;

            // Map hash → random point on unit sphere
            float theta = (h & 0xFFFF) / 65535f * MathF.Tau;
            float phi = ((h >> 16) & 0xFFFF) / 65535f * MathF.PI;

            float sinPhi = MathF.Sin(phi);

            return new Vector3(
                MathF.Cos(theta) * sinPhi,
                MathF.Sin(theta) * sinPhi,
                MathF.Cos(phi)
            );
        }

        public float GetNoise(Vector3 vec)
        {
            int x0 = (int)MathF.Floor(vec.X);
            int y0 = (int)MathF.Floor(vec.Y);
            int z0 = (int)MathF.Floor(vec.Z);

            int x1 = x0 + 1;
            int y1 = y0 + 1;
            int z1 = z0 + 1;

            Vector3 d000 = vec - new Vector3(x0, y0, z0);
            Vector3 d100 = vec - new Vector3(x1, y0, z0);
            Vector3 d010 = vec - new Vector3(x0, y1, z0);
            Vector3 d110 = vec - new Vector3(x1, y1, z0);
            Vector3 d001 = vec - new Vector3(x0, y0, z1);
            Vector3 d101 = vec - new Vector3(x1, y0, z1);
            Vector3 d011 = vec - new Vector3(x0, y1, z1);
            Vector3 d111 = vec - new Vector3(x1, y1, z1);

            float n000 = Vector3.Dot(Grad(x0, y0, z0), d000);
            float n100 = Vector3.Dot(Grad(x1, y0, z0), d100);
            float n010 = Vector3.Dot(Grad(x0, y1, z0), d010);
            float n110 = Vector3.Dot(Grad(x1, y1, z0), d110);
            float n001 = Vector3.Dot(Grad(x0, y0, z1), d001);
            float n101 = Vector3.Dot(Grad(x1, y0, z1), d101);
            float n011 = Vector3.Dot(Grad(x0, y1, z1), d011);
            float n111 = Vector3.Dot(Grad(x1, y1, z1), d111);

            float u = Fade(d000.X);
            float v = Fade(d000.Y);
            float w = Fade(d000.Z);

            float nx00 = Lerp(n000, n100, u);
            float nx10 = Lerp(n010, n110, u);
            float nx01 = Lerp(n001, n101, u);
            float nx11 = Lerp(n011, n111, u);

            float nxy0 = Lerp(nx00, nx10, v);
            float nxy1 = Lerp(nx01, nx11, v);

            return Lerp(nxy0, nxy1, w);
        }

        public float Fractal(Vector3 vec, int octaves, float lacunarity, float persistence)
        {
            float sum = 0f;
            float amp = 1f;
            float freq = 1f;
            float max = 0f;

            for (int i = 0; i < octaves; i++)
            {
                sum += GetNoise(vec * freq) * amp;

                max += amp;
                amp *= persistence;
                freq *= lacunarity;
            }

            return sum / max;
        }
    }
}
