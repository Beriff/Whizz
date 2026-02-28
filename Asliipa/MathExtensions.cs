using System;
using System.Collections.Generic;
using System.Text;

namespace Whizz
{
    public static class MathExtensions
    {
        extension(float a)
        {
            public bool FuzzyEq(float b, float tolerance = float.Epsilon) => MathF.Abs(a - b) < tolerance;
        }
    }
}
