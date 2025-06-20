using System;
using System.Numerics;

namespace ArxLibertatisLightingCalculatorLib.Util
{
    public static class RandomHelper
    {
        static readonly Random rand = new Random();

        public static float RandomValueNormalDistribution(float mean = 0, float stdDev = 1)
        {
            float u1 = 1.0f - (float)rand.NextDouble(); //uniform(0,1] random doubles
            float u2 = 1.0f - (float)rand.NextDouble();
            float randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Sin(2.0f * MathF.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }

        public static Vector3 RandomDirection()
        {
            float x = RandomValueNormalDistribution(0, 1);
            float y = RandomValueNormalDistribution(0, 1);
            float z = RandomValueNormalDistribution(0, 1);
            var vec = new Vector3(x, y, z);
            return Vector3.Normalize(vec);
        }

        public static Vector3 RandomHemisphereDirection(Vector3 normal)
        {
            var dir = RandomDirection();
            return dir * MathF.Sign(Vector3.Dot(normal, dir));
        }
    }
}
