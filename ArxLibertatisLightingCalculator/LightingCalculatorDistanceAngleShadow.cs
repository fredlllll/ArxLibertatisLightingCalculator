using ArxLibertatisEditorIO.Util;

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
