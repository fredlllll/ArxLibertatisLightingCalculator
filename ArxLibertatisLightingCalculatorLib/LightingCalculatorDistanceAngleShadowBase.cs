using ArxLibertatisEditorIO.MediumIO.Shared;
using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.Util;
using System;
using System.Numerics;
using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisLightingCalculatorLib.RayCasting;

namespace ArxLibertatisLightingCalculatorLib
{
    public abstract class LightingCalculatorDistanceAngleShadowBase : LightingCalculatorBase
    {
        public abstract PolyType[] GetPolyTypesToSkip();
        protected IRaycastProvider raycastProvider;

        public LightingCalculatorDistanceAngleShadowBase(IRaycastProvider raycastProvider)
        {
            this.raycastProvider = raycastProvider;
        }

        public override void Calculate(MediumArxLevel mal)
        {
            //init simulation
            raycastProvider.Initialize(this, mal);
            base.Calculate(mal);
        }

        public override Color CalculateVertex(Vertex v, Polygon p, bool doubleSided)
        {
            Color col = new Color(0, 0, 0);

            foreach (var l in dynLights)
            {
                Color lightColor;
                if (false && l.extras.HasFlag(ExtrasType.EXTRAS_NOCASTED)) // don't calculate shadows for this light, no shadow cast
                {
                    lightColor = NoCasted(v, l, doubleSided);
                }
                else
                {
                    lightColor = CalculateLight(v, l, doubleSided);
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

        private Color NoCasted(Vertex v, Light l, bool doubleSided)
        {
            Vector3 lightPos = l.pos + scenePos;
            Vector3 lightVector = lightPos - v.position; //vector pointing from vertex to light
            float dist = lightVector.Length();
            if (dist > l.fallEnd) //outside of light range
            {
                return new Color(0, 0, 0);
            }
            lightVector /= dist; //normalize
            float factorAngle;
            if (doubleSided)
            {
                factorAngle = MathF.Abs(Vector3.Dot(v.normal, lightVector));
            }
            else
            {
                factorAngle = Vector3.Dot(v.normal, lightVector);
                if (factorAngle <= 0) //not facing light
                {
                    return new Color(0, 0, 0);
                }
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

        private Color CalculateLight(Vertex v, Light l, bool doubleSided)
        {

            Vector3 lightPos = l.pos + scenePos;
            Vector3 lightVector = lightPos - v.position; //vector pointing from vertex to light
            float dist = lightVector.Length();
            if (dist > l.fallEnd) //outside of light range
            {
                return new Color(0, 0, 0);
            }

            //var hitHandler = new HitHandler(null);
            //sim.RayCast(lightPos, -lightVector, 0.99f, ref hitHandler);
            var raycastHit = raycastProvider.Raycast(lightPos, -lightVector, 0.99f);
            if (raycastHit.HitCount > 0) //in the shadow
            {
                return new Color(0, 0, 0);
            }

            lightVector /= dist; //normalize
            float factorAngle;
            if (doubleSided)
            {
                factorAngle = MathF.Abs(Vector3.Dot(v.normal, lightVector));
            }
            else
            {
                factorAngle = Vector3.Dot(v.normal, lightVector);
                if (factorAngle <= 0) //not facing light
                {
                    return new Color(0, 0, 0);
                }
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
