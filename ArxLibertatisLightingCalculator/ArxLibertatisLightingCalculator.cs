using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisLightingCalculator.GI;

namespace ArxLibertatisLightingCalculator
{
    public static class ArxLibertatisLightingCalculator
    {
        public static void Calculate(MediumArxLevel mal, LightingProfile lightingProfile)
        {
            ILightingCalculator calculator = null;
            switch (lightingProfile)
            {
                case LightingProfile.Distance:
                    calculator = new LightingCalculatorDistance();
                    break;
                case LightingProfile.Danae:
                    calculator = new LightingCalculatorDanae();
                    break;
                case LightingProfile.DistanceAngle:
                    calculator = new LightingCalculatorDistanceAngle();
                    break;
                case LightingProfile.DistanceAngleShadow:
                    calculator = new LightingCalculatorDistanceAngleShadow();
                    break;
                case LightingProfile.DistanceAngleShadowNoTransparency:
                    calculator = new LightingCalculatorDistanceAngleShadowNoTransparency();
                    break;
                case LightingProfile.GI:
                    calculator = new LightingCalculatorGI();
                    break;
            }
            calculator.Calculate(mal);
        }
    }
}
