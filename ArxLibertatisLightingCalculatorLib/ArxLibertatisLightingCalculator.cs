using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisLightingCalculatorLib.GI;
using System;

namespace ArxLibertatisLightingCalculatorLib
{
    public static class ArxLibertatisLightingCalculator
    {
        public static void Calculate(MediumArxLevel mal, LightingProfile lightingProfile)
        {
            ILightingCalculator calculator = lightingProfile switch
            {
                LightingProfile.Distance => new LightingCalculatorDistance(),
                LightingProfile.Danae => new LightingCalculatorDanae(),
                LightingProfile.DistanceAngle => new LightingCalculatorDistanceAngle(),
                LightingProfile.DistanceAngleShadow => new LightingCalculatorDistanceAngleShadow(),
                LightingProfile.DistanceAngleShadowNoTransparency => new LightingCalculatorDistanceAngleShadowNoTransparency(),
                LightingProfile.GI => new LightingCalculatorGI(),
                _ => throw new InvalidOperationException("invalid lighting profile " + lightingProfile),
            };
            calculator.Calculate(mal);
        }
    }
}
