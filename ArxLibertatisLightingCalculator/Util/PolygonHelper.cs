using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.Util;
using System.Numerics;

namespace ArxLibertatisLightingCalculator.Util
{
    public static class PolygonHelper
    {
        public static float CalculatePolygonArea(Polygon p)
        {
            if (p.polyType.HasFlag(PolyType.QUAD))
            {
                return CalculateQuadSize(p.vertices[0].position, p.vertices[1].position, p.vertices[2].position, p.vertices[3].position);
            }
            return CalculateTriangleArea(p.vertices[0].position, p.vertices[1].position, p.vertices[2].position);
        }

        public static float CalculateTriangleArea(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 crossProduct = Vector3.Cross(v2 - v1, v3 - v1);
            float area = 0.5f * crossProduct.Length();

            return area;
        }

        public static float CalculateQuadSize(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            float triangle1Area = CalculateTriangleArea(v1, v2, v3);
            float triangle2Area = CalculateTriangleArea(v1, v3, v4);
            return triangle1Area + triangle2Area;
        }
    }
}
