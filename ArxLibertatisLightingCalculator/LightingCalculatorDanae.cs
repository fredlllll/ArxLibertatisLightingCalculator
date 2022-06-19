using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDanae : LightingCalculatorBase
    {
        float globalLightFactor = 0.45f;
        //float ambientColor = 35f / 255; //for npc and item
        float ambientColor = 0.09f; //for level

        public override Color CalculateVertex(Vertex v)
        {
            Color col = new Color(ambientColor, ambientColor, ambientColor);

            foreach (var l in dynLights)
            {
                float cosangle;
                Vector3 lightPos = scenePos + l.pos;
                float distance = Vector3.Distance(v.position, lightPos);

                /* Evaluate its intensity depending on the distance Light<->Object */
                if (distance <= l.fallStart)
                    cosangle = l.intensity * globalLightFactor;
                else
                {
                    float fallDiffMul = 1 / (l.fallEnd - l.fallStart);
                    float p = ((l.fallEnd - distance) * fallDiffMul);

                    if (p <= 0)
                        cosangle = 0;
                    else
                        cosangle = p * l.intensity * globalLightFactor;
                }
                col.r += l.color.r * cosangle;
                col.g += l.color.g * cosangle;
                col.b += l.color.b * cosangle;
            }

            col.r = MathF.Min(col.r, 1);
            col.g = MathF.Min(col.g, 1);
            col.b = MathF.Min(col.b, 1);
            return col;
        }
    }
}
