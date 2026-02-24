using System;
using System.Numerics;

namespace Whizz
{
    public class Noise(int seed)
    {
        private readonly int Seed = seed;

        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        private static float Lerp(float a, float b, float t) => a + t * (b - a);

        private Vector2 Grad(int x, int y)
        {
            int h = x * 374761393 + y * 668265263 + Seed * 1442695041;

            h = (h ^ (h >> 13)) * 1274126177;
            h ^= h >> 16;

            float angle = (h & 0xFFFF) / 65535f * MathF.Tau;
            return new(MathF.Cos(angle), MathF.Sin(angle));
        }
        
        public float GetNoise(Vector2 vec)
        {
            int x0 = (int)MathF.Floor(vec.X);
            int y0 = (int)MathF.Floor(vec.Y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            Vector2 d00 = new(vec.X - x0, vec.Y - y0);
            Vector2 d10 = new(vec.X - x1, vec.Y - y0);
            Vector2 d01 = new(vec.X - x0, vec.Y - y1);
            Vector2 d11 = new(vec.X - x1, vec.Y - y1);

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
