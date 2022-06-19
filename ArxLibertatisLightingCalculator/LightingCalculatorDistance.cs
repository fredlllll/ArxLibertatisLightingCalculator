using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using System;
using System.Numerics;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDistance : LightingCalculatorBase
    {
        public override Color CalculateVertex(Vertex v, Polygon p)
        {
            Color col = new Color(0, 0, 0);

            foreach (var l in dynLights)
            {
                Vector3 lightPos = l.pos + scenePos;
                float dist = Vector3.Distance(v.position, lightPos);
                if (dist > l.fallEnd)
                {
                    continue;
                }
                float factor = 1;
                if (dist > l.fallStart)
                {
                    float diff = l.fallEnd - l.fallStart;
                    factor = 1-((dist - l.fallStart) / diff);
                    factor *= factor; //quadratic falloff
                }
                factor *= l.intensity;

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
