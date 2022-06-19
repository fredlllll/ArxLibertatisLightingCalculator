﻿using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.MediumIO.Shared;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisEditorIO.WellDoneIO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDistanceAngle : LightingCalculatorBase
    {
        public override Color CalculateVertex(ArxLibertatisEditorIO.MediumIO.FTS.Vertex v)
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
