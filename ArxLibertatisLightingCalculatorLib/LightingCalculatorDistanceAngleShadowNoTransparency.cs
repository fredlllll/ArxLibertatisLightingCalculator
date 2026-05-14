using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculatorLib.RayCasting;

namespace ArxLibertatisLightingCalculatorLib
{
    public class LightingCalculatorDistanceAngleShadowNoTransparency : LightingCalculatorDistanceAngleShadowBase
    {
        public LightingCalculatorDistanceAngleShadowNoTransparency(IRaycastProvider raycastProvider) : base(raycastProvider)
        {
        }

        private readonly PolyType[] polyTypesToSkip = new PolyType[]
        {
            PolyType.TRANS,
            PolyType.WATER,
            PolyType.LAVA,
            PolyType.NODRAW,
            PolyType.NO_SHADOW,
            PolyType.CLIMB,
        };

        public override PolyType[] GetPolyTypesToSkip()
        {
            return polyTypesToSkip;
        }
    }
}
