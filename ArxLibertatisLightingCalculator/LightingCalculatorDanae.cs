using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using System;
using System.Numerics;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDanae : LightingCalculatorBase
    {
        float globalLightFactor = 0.85f; //was 0.85f
        //float ambientColor = 35f / 255; //for npc and item
        float ambientColor = 0.09f; //for level

        public override Color CalculateVertex(Vertex v, Polygon poly)
        {
            //Color col = new Color(ambientColor, ambientColor, ambientColor);
            Color col = new Color(0, 0, 0);

            foreach (var l in dynLights)
            {
                Vector3 lightPos = l.pos + scenePos;

                float cosangle;
                Vector3 tl = new Vector3();
                tl.X = (lightPos.X - v.position.X);
                tl.Y = (lightPos.Y - v.position.Y);
                tl.Z = (lightPos.Z - v.position.Z);
                float dista = tl.Length();

                if (dista < l.fallEnd)
                {
                    float divv = 1f / dista;
                    tl.X *= divv;
                    tl.Y *= divv;
                    tl.Z *= divv;

                    //VectorMatrixMultiply(Cur_vTLights, &tl, &matrix);
                    Vector3 lightVector = Vector3.Normalize(lightPos -v.position); // Cur_vTLights
                    v.normal = Vector3.Normalize(v.normal);

                    cosangle = (v.normal.X * lightVector.X +
                                v.normal.Y * lightVector.Y +
                                v.normal.Z * lightVector.Z);

                    /* If light visible */
                    if (cosangle > 0.0f)
                    {
                        float precalc = l.intensity * globalLightFactor;
                        float fallDiffMul = 1 / (l.fallEnd - l.fallStart);
                        /* Evaluate its intensity depending on the distance Light<->Object */
                        if (dista <= l.fallStart)
                            cosangle *= precalc;
                        else
                        {
                            float p = ((l.fallEnd - dista) * fallDiffMul);

                            if (p <= 0)
                                cosangle = 0;
                            else
                                cosangle *= p * precalc;
                        }

                        col.r += l.color.r * cosangle;
                        col.g += l.color.g * cosangle;
                        col.b += l.color.b * cosangle;
                    }
                }

            }

            col.r = MathF.Min(col.r, 1);
            col.g = MathF.Min(col.g, 1);
            col.b = MathF.Min(col.b, 1);
            return col;
        }
    }
}
