using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.MediumIO.FTS;
using ArxLibertatisEditorIO.MediumIO.Shared;
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
    public class LightingCalculatorDistanceAngleShadow : LightingCalculatorDistanceAngleShadowBase
    {
        private readonly PolyType[] polyTypesToSkip = new PolyType[]
        {
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
