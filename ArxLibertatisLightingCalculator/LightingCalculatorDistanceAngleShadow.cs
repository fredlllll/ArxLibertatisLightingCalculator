using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.MediumIO.Shared;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculator.Bepu;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDistanceAngleShadow : LightingCalculatorDistanceAngle
    {
        Simulation sim;
        BufferPool pool;

        public override void Calculate(MediumArxLevel mal)
        {
            pool = new BufferPool();
            sim = Simulation.Create(pool, new NoCollisionCallbacks(), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new PositionFirstTimestepper());

            var triangles = new List<Triangle>();
            var triangleBuffer = new Buffer<Triangle>();

            for (int i = 0; i < mal.FTS.cells.Count; ++i)
            {
                var c = mal.FTS.cells[i];
                for (int j = 0; j < c.polygons.Count; ++j)
                {
                    var p = c.polygons[j];

                    var t = new Triangle(p.vertices[0].position, p.vertices[1].position, p.vertices[2].position);
                    triangles.Add(t);
                    if (p.polyType.HasFlag(PolyType.QUAD))
                    {
                        t = new Triangle(p.vertices[2].position, p.vertices[1].position, p.vertices[3].position);
                        triangles.Add(t);
                    }
                }
            }
            var trianglesArray = triangles.ToArray();
            triangleBuffer.CopyFrom(new Span<Triangle>(trianglesArray), 0, 0, trianglesArray.Length);

            var mesh = new Mesh(triangleBuffer, Vector3.One, pool);
            sim.Statics.Add(new StaticDescription(
                new Vector3(0, -10, 0), QuaternionEx.CreateFromAxisAngle(new Vector3(0, 1, 0), MathF.PI / 4),
                new CollidableDescription(sim.Shapes.Add(mesh), 0.1f)));

            base.Calculate(mal);
        }

        public override Color CalculateVertex(Vertex v, Polygon p)
        {
            Color col = new Color(0, 0, 0);

            foreach (var l in dynLights)
            {
                Color lightColor;
                if (l.extras.HasFlag(ExtrasType.EXTRAS_NOCASTED))
                {
                    lightColor = NoCasted(v, l);
                }
                else
                {
                    lightColor = CalculateLight(v, l);
                }

                col.r += lightColor.r;
                col.g += lightColor.g;
                col.b += lightColor.b;
            }

            col.r = MathF.Min(col.r, 1);
            col.g = MathF.Min(col.g, 1);
            col.b = MathF.Min(col.b, 1);
            return col;
        }

        private Color NoCasted(Vertex v, Light l)
        {
            Vector3 lightPos = l.pos + scenePos;
            Vector3 lightVector = lightPos - v.position; //vector pointing from vertex to light
            float dist = lightVector.Length();
            if (dist > l.fallEnd) //outside of light range
            {
                return new Color(0, 0, 0);
            }
            lightVector /= dist; //normalize
            float factorAngle = Vector3.Dot(v.normal, lightVector);
            if (factorAngle <= 0) //not facing light
            {
                return new Color(0, 0, 0);
            }
            float factorDistance = 1;
            if (dist > l.fallStart)
            {
                float diff = l.fallEnd - l.fallStart;
                factorDistance = 1 - ((dist - l.fallStart) / diff);
            }
            float factor = factorDistance * factorAngle * l.intensity;

            return new Color(l.color.r * factor, l.color.g * factor, l.color.b * factor);
        }

        private Color CalculateLight(Vertex v, Light l)
        {
            Vector3 lightPos = l.pos + scenePos;
            Vector3 lightVector = lightPos - v.position; //vector pointing from vertex to light
            float dist = lightVector.Length();
            if (dist > l.fallEnd) //outside of light range
            {
                return new Color(0, 0, 0);
            }
            
            var hitHandler = new HitHandler();
            sim.RayCast(v.position, lightVector, 2, ref hitHandler);
            if(hitHandler.hits.Count > 0) //in the shadow
            {
                return new Color(0, 0, 0);
            }

            lightVector /= dist; //normalize
            float factorAngle = Vector3.Dot(v.normal, lightVector);
            if (factorAngle <= 0) //not facing light
            {
                return new Color(0, 0, 0);
            }
            float factorDistance = 1;
            if (dist > l.fallStart)
            {
                float diff = l.fallEnd - l.fallStart;
                factorDistance = 1 - ((dist - l.fallStart) / diff);
            }
            float factor = factorDistance * factorAngle * l.intensity;

            return new Color(l.color.r * factor, l.color.g * factor, l.color.b * factor);
        }
    }
}
