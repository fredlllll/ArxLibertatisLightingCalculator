using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using System.Collections.Generic;
using System.Numerics;

namespace ArxLibertatisLightingCalculator.Bepu
{
    struct RayHit
    {
        public Vector3 Normal;
        public float T;
        public CollidableReference Collidable;
        public bool Hit;
    }

    struct HitHandler : IRayHitHandler
    {
        public List<RayHit> hits;

        public HitHandler(List<RayHit> hits = null)
        {
            if (hits == null)
            {
                hits = new List<RayHit>();
            }
            this.hits = hits;
        }

        //public Buffer<RayHit> Hits;
        public bool AllowTest(CollidableReference collidable)
        {
            return true;
        }

        public bool AllowTest(CollidableReference collidable, int childIndex)
        {
            return true;
        }

        public void OnRayHit(in RayData ray, ref float maximumT, float t, in Vector3 normal, CollidableReference collidable, int childIndex)
        {
            maximumT = t;
            var hit = new RayHit();
            hit.Normal = normal;
            hit.T = t;
            hit.Collidable = collidable;
            hit.Hit = true;
            hits.Add(hit);
        }
    }
}
