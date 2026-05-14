using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculatorLib.RayCasting;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ArxLibertatisLightingCalculatorLib.Bepu
{
    public class BepuRaycastProvider : IRaycastProvider
    {
        protected Simulation? sim;
        protected BufferPool? pool;

        bool SkipPolygon(LightingCalculatorDistanceAngleShadowBase lc, PolyType pt)
        {
            var polyTypesToSkip = lc.GetPolyTypesToSkip();
            for (int k = 0; k < polyTypesToSkip.Length; ++k)
            {
                if (pt.HasFlag(polyTypesToSkip[k]))
                {
                    return true;
                }
            }
            return false;
        }

        public void Initialize(LightingCalculatorDistanceAngleShadowBase lc, MediumArxLevel mal)
        {
            if (sim != null) // Just in case, dispose the previous simulation if it exists.
            {
                sim.Dispose();
                sim = null;
            }
            pool = new BufferPool();
            sim = Simulation.Create(pool, new NoCollisionCallbacks(), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new PositionFirstTimestepper());

            var triangles = new List<Triangle>();


            for (int i = 0; i < mal.FTS.cells.Count; ++i)
            {
                var c = mal.FTS.cells[i];
                for (int j = 0; j < c.polygons.Count; ++j)
                {
                    var p = c.polygons[j];
                    if (SkipPolygon(lc, p.polyType))
                    {
                        continue;
                    }

                    var t = new Triangle(p.vertices[0].position, p.vertices[1].position, p.vertices[2].position);
                    triangles.Add(t);
                    t = new Triangle(p.vertices[0].position, p.vertices[2].position, p.vertices[1].position);
                    triangles.Add(t);
                    if (p.polyType.HasFlag(PolyType.QUAD))
                    {
                        t = new Triangle(p.vertices[1].position, p.vertices[2].position, p.vertices[3].position);
                        triangles.Add(t);
                        t = new Triangle(p.vertices[2].position, p.vertices[1].position, p.vertices[3].position);
                        triangles.Add(t);
                    }
                }
            }
            var trianglesArray = triangles.ToArray();
            pool.Take(trianglesArray.Length, out Buffer<Triangle> triangleBuffer);
            triangleBuffer.CopyFrom(new Span<Triangle>(trianglesArray), 0, 0, trianglesArray.Length);

            var mesh = new Mesh(triangleBuffer, Vector3.One, pool);
            sim.Statics.Add(new StaticDescription(
                new Vector3(0, 0, 0), QuaternionEx.Identity,
                new CollidableDescription(sim.Shapes.Add(mesh), 0.1f)));
        }

        public RaycastHitData Raycast(Vector3 origin, Vector3 direction, float maximumT)
        {
            var hitHandler = new HitHandler(null);
            sim!.RayCast(origin, direction, maximumT, ref hitHandler);

            return new RaycastHitData
            {
                HitCount = hitHandler.hits.Count
            };
        }
    }
}
