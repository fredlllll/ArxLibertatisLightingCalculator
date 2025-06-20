using ArxLibertatisEditorIO.Util;

namespace ArxLibertatisLightingCalculatorLib
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
