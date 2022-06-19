using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.Util;
using ArxLibertatisEditorIO.WellDoneIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArxLibertatisLightingCalculator
{
    public interface ILightingCalculator
    {
        void Calculate(MediumArxLevel mal);
    }
}
