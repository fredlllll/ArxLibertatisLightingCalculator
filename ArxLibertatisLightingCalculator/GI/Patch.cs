using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculator.Util;
using BepuPhysics.Collidables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ArxLibertatisLightingCalculator.GI
{
    public class Patch
    {
        public Vector3 position;
        public Vector3 normal;
        public Color emission;
        public Color reflectivity;
        public Color radiosity, radiosityTmp;
        public float size;

        public Patch Clone()
        {
            return new Patch()
            {
                position = position,
                normal = normal,
                emission = emission,
                reflectivity = reflectivity,
                radiosity = radiosity,
                radiosityTmp = radiosityTmp,
                size = size
            };
        }

        public override string ToString()
        {
            return $"<Pos:{position},Nor:{normal},Emi:{emission},Ref:{reflectivity},Rad:{radiosity},Siz:{size}>";
        }

        public static Patch FromPolygon(Polygon p, ref int vertIndex, List<Color> vertexLightColors)
        {
            Patch patch = new Patch();

            int vertCount = 3;
            Color color = vertexLightColors[vertIndex++] + vertexLightColors[vertIndex++] + vertexLightColors[vertIndex++];
            Vector3 pos = p.vertices[0].position + p.vertices[1].position + p.vertices[2].position;
            Vector3 norm = p.vertices[0].normal + p.vertices[1].normal + p.vertices[2].normal;

            if (p.polyType.HasFlag(PolyType.QUAD))
            {
                color += vertexLightColors[vertIndex++];
                pos += p.vertices[3].position;
                norm += p.vertices[3].normal;
                vertCount++;
            }
            patch.position = pos / vertCount;
            patch.normal = Vector3.Normalize(norm);
            patch.size = PolygonHelper.CalculatePolygonArea(p);

            // Set the material color as the patch reflectivity
            patch.radiosity = color / vertCount; //TODO: could load texture of polygon and use the mean value of that too
            patch.reflectivity = patch.radiosity / 10;
            if (p.polyType.HasFlag(PolyType.GLOW))
            {
                patch.emission = patch.radiosity / 2;
            }
            else
            {
                //patch.emission = patch.radiosity / 5;
            }

            return patch;
        }
    }
}
