using SDL2;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Whizz
{
    public class Noise3D(int seed)
    {
        private readonly int Seed = seed;

        private static float Fade(float t)
            => t * t * t * (t * (t * 6 - 15) + 10);

        private static float Lerp(float a, float b, float t)
            => a + t * (b - a);

        private static readonly Vector3[] Gradients =
        [
            new(1,1,0), new(-1,1,0), new(1,-1,0), new(-1,-1,0),
            new(1,0,1), new(-1,0,1), new(1,0,-1), new(-1,0,-1),
            new(0,1,1), new(0,-1,1), new(0,1,-1), new(0,-1,-1)
        ];

        private Vector3 Grad(int x, int y, int z)
        {
            int h =
                x * 374761393 ^
                y * 668265263 ^
                z * 2147483647 ^
                Seed * 1442695041;

            h = (h ^ (h >> 13)) * 1274126177;
            h ^= h >> 16;

            int index = (h & int.MaxValue) % Gradients.Length;
            return Gradients[index];
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

    public class Noise2D(int seed)
    {
        private readonly int Seed = seed;

        private static float Fade(float t)
            => t * t * t * (t * (t * 6 - 15) + 10);

        private static float Lerp(float a, float b, float t)
            => a + t * (b - a);

        private static readonly Vector2[] Gradients =
        [
            new( 1, 0), new(-1, 0),
            new( 0, 1), new( 0,-1),
            new( 0.7071f,  0.7071f),
            new(-0.7071f,  0.7071f),
            new( 0.7071f, -0.7071f),
            new(-0.7071f, -0.7071f)
        ];

        private Vector2 Grad(int x, int y)
        {
            int h =
                x * 374761393 ^
                y * 668265263 ^
                Seed * 1442695041;

            h = (h ^ (h >> 13)) * 1274126177;
            h ^= h >> 16;

            int index = (h & int.MaxValue) % Gradients.Length;
            return Gradients[index];
        }

        public float GetNoise(Vector2 vec)
        {
            int x0 = (int)MathF.Floor(vec.X);
            int y0 = (int)MathF.Floor(vec.Y);

            int x1 = x0 + 1;
            int y1 = y0 + 1;

            Vector2 d00 = vec - new Vector2(x0, y0);
            Vector2 d10 = vec - new Vector2(x1, y0);
            Vector2 d01 = vec - new Vector2(x0, y1);
            Vector2 d11 = vec - new Vector2(x1, y1);

            float n00 = Vector2.Dot(Grad(x0, y0), d00);
            float n10 = Vector2.Dot(Grad(x1, y0), d10);
            float n01 = Vector2.Dot(Grad(x0, y1), d01);
            float n11 = Vector2.Dot(Grad(x1, y1), d11);

            float u = Fade(d00.X);
            float v = Fade(d00.Y);

            float nx0 = Lerp(n00, n10, u);
            float nx1 = Lerp(n01, n11, u);

            return Lerp(nx0, nx1, v);
        }

        public float Fractal(Vector2 vec, int octaves, float lacunarity, float persistence)
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
