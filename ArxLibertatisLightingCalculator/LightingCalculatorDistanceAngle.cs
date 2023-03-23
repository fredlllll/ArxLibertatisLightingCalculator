using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using System;
using System.Numerics;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDistanceAngle : LightingCalculatorBase
    {
        public override Color CalculateVertex(Vertex v, Polygon p)
        {
            Color col = new Color(0, 0, 0);

            foreach (var l in dynLights)
            {
                Vector3 lightPos = l.pos + scenePos;
                Vector3 lightVector = lightPos - v.position; //vector pointing from vertex to light
                float dist = lightVector.Length();
                if (dist > l.fallEnd) //outside of light range
                {
                    continue;
                }
                lightVector /= dist; //normalize
                float factorAngle = Vector3.Dot(v.normal, lightVector);
                if (factorAngle <= 0) //not facing light
                {
                    continue;
                }
                float factorDistance = 1;
                if (dist > l.fallStart)
                {
                    float diff = l.fallEnd - l.fallStart;
                    factorDistance = 1 - ((dist - l.fallStart) / diff);
                }
                float factor = factorDistance * factorAngle * l.intensity;

                col.r += l.color.r * factor;
                col.g += l.color.g * factor;
                col.b += l.color.b * factor;
            }

            col.r = MathF.Min(col.r, 1);
            col.g = MathF.Min(col.g, 1);
            col.b = MathF.Min(col.b, 1);
            return col;
        }
    }
}
