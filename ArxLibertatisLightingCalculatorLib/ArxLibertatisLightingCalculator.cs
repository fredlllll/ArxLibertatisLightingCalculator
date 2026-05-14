using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisLightingCalculatorLib.GI;
using ArxLibertatisLightingCalculatorLib.RayCasting;
using System;

namespace ArxLibertatisLightingCalculatorLib
{
    public static class ArxLibertatisLightingCalculator
    {
        public static void Calculate(MediumArxLevel mal, LightingProfile lightingProfile, IRaycastProvider? raycastProvider = null)
        {
            ILightingCalculator calculator = lightingProfile switch
            {
                LightingProfile.Distance => new LightingCalculatorDistance(),
                LightingProfile.Danae => new LightingCalculatorDanae(),
                LightingProfile.DistanceAngle => new LightingCalculatorDistanceAngle(),
                LightingProfile.DistanceAngleShadow => new LightingCalculatorDistanceAngleShadow(raycastProvider ?? throw new ArgumentNullException("need raycast provider")),
                LightingProfile.DistanceAngleShadowNoTransparency => new LightingCalculatorDistanceAngleShadowNoTransparency(raycastProvider ?? throw new ArgumentNullException("need raycast provider")),
                LightingProfile.GI => new LightingCalculatorGI(raycastProvider ?? throw new ArgumentNullException("need raycast provider")),
                _ => throw new InvalidOperationException("invalid lighting profile " + lightingProfile),
            };
            calculator.Calculate(mal);
        }
    }
}
