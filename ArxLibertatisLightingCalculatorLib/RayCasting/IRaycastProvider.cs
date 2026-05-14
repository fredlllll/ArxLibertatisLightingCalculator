using ArxLibertatisEditorIO.MediumIO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ArxLibertatisLightingCalculatorLib.RayCasting
{
    public interface IRaycastProvider
    {
        void Initialize(LightingCalculatorDistanceAngleShadowBase lc, MediumArxLevel mal);
        /// <summary>
        /// </summary>
        /// <param name="origin">Origin of the ray to cast.</param>
        /// <param name="direction">Direction of the ray to cast.</param>
        /// <param name="maximumT">Maximum length of the ray traversal in units of the direction's length.</param>
        /// <returns></returns>
        RaycastHitData Raycast(Vector3 origin, Vector3 direction, float maximumT);
    }
}
