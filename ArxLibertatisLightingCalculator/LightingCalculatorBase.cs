using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.MediumIO.Shared;
using ArxLibertatisEditorIO.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArxLibertatisLightingCalculator
{
    public abstract class LightingCalculatorBase : ILightingCalculator
    {
        protected readonly List<Light> dynLights = new List<Light>();

        protected Vector3 scenePos;

        public void Calculate(MediumArxLevel mal)
        {
            //extract dynamic lights
            dynLights.Clear();
            foreach (var l in mal.LLF.lights)
            {
                if (!l.extras.HasFlag(ExtrasType.EXTRAS_SEMIDYNAMIC))
                {
                    dynLights.Add(l);
                }
            }

            scenePos = mal.FTS.sceneHeader.Mscenepos;

            int vertIndex = 0;
            for (int i = 0; i < mal.FTS.cells.Count; ++i)
            {
                var c = mal.FTS.cells[i];
                for (int j = 0; j < c.polygons.Count; ++j)
                {
                    var p = c.polygons[j];
                    for (int k = 0; k < p.VertexCount; ++k)
                    {
                        mal.LLF.lightColors[vertIndex++] = CalculateVertex(p.vertices[k]);
                    }
                }
            }
        }

        public abstract Color CalculateVertex(Vertex v);
    }
}
