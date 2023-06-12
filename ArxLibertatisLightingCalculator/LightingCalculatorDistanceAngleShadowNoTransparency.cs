using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculator.Bepu;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using BepuUtilities.Memory;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ArxLibertatisLightingCalculator
{
    public class LightingCalculatorDistanceAngleShadowNoTransparency : LightingCalculatorDistanceAngleShadowBase
    {
        private readonly PolyType[] polyTypesToSkip = new PolyType[]
        {
            PolyType.TRANS,
            PolyType.WATER,
            PolyType.LAVA,
            PolyType.NODRAW,
            PolyType.NO_SHADOW,
            PolyType.CLIMB,
        };

        protected override PolyType[] GetPolyTypesToSkip()
        {
            return polyTypesToSkip;
        }
    }
}
