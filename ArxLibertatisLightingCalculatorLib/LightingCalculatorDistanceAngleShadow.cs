using ArxLibertatisEditorIO.Util;
using ArxLibertatisLightingCalculatorLib.RayCasting;

namespace ArxLibertatisLightingCalculatorLib
{   
    public class LightingCalculatorDistanceAngleShadow : LightingCalculatorDistanceAngleShadowBase
    {
        public LightingCalculatorDistanceAngleShadow(IRaycastProvider raycastProvider) : base(raycastProvider)
        {
        }

        private readonly PolyType[] polyTypesToSkip = new PolyType[]
        {
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
