using Plexdata.ArgumentParser.Attributes;
using Plexdata.ArgumentParser.Interfaces;

namespace ArxLibertatisLightingCalculator
{
    [HelpUtilize]
    [ParametersGroup]
    public class CommandLineArgs
    {
        [HelpSummary("Name of the level, e.g. \"level1\". Use together with arx-data-dir")]
        [OptionParameter(SolidLabel = "level", DependencyList = "ArxDataDir", DefaultValue = null)]
        public string Level { get; set; }

        [HelpSummary("Path to the arx data dir, so we can find the level by name. The one containing the folders editor, game, graph, localisation and misc. Required if using the level option")]
        [OptionParameter(SolidLabel = "arx-data-dir", DefaultValue = null)]
        public string ArxDataDir { get; set; }

        [HelpSummary("The path to the dlf file, requires fts and llf")]
        [OptionParameter(SolidLabel = "dlf", DependencyList = "Fts,Llf", DefaultValue = null)]
        public string Dlf { get; set; }

        [HelpSummary("The path to the fts file, requires dlf and llf")]
        [OptionParameter(SolidLabel = "fts", DependencyList = "Dlf,Llf", DefaultValue = null)]
        public string Fts { get; set; }

        [HelpSummary("The path to the llf file, requires fts and dlf")]
        [OptionParameter(SolidLabel = "llf", DependencyList = "Fts,Dlf", DefaultValue = null)]
        public string Llf { get; set; }

        [HelpSummary("What lighting profile to use. Possible Values: " + nameof(LightingProfile.Distance) + ", " + nameof(LightingProfile.Danae) + ", " + nameof(LightingProfile.DistanceAngle) + ", " + nameof(LightingProfile.DistanceAngleShadow))]
        [OptionParameter(SolidLabel = "lighting-profile")]
        [CustomConverter(typeof(CustomConverter))]
        public LightingProfile LightingProfile
        {
            get; set;
        } = LightingProfile.Danae;
    }

    public class CustomConverter : ICustomConverter<LightingProfile>
    {
        public LightingProfile Convert(string parameter, string argument, string delimiter)
        {
            if (argument is null) { return LightingProfile.Danae; }

            return (LightingProfile)System.Enum.Parse(typeof(LightingProfile), argument);
        }
    }
}
