using CommandLine.Text;
using CommandLine;
using System.Collections.Generic;

namespace ArxLibertatisLightingCalculator
{
    public class CommandLineArgs
    {
        [Option("level", Required = false, HelpText = "Name of the level, e.g. \"level1\". Use together with arx-data-dir", Default = null)]
        public string Level { get; set; }

        [Option("arx-data-dir", Required = false, HelpText = "Path to the arx data dir, so we can find the level by name. The one containing the folders editor, game, graph, localisation and misc. Required if using the level option", Default = null)]
        public string ArxDataDir { get; set; }

        [Option("dlf", Required = false, HelpText = "The path to the dlf file, requires fts and llf", Default = null)]
        public string Dlf { get; set; }

        [Option("fts", Required = false, HelpText = "The path to the fts file, requires dlf and llf", Default = null)]
        public string Fts { get; set; }

        [Option("llf", Required = false, HelpText = "The path to the llf file, requires fts and dlf", Default = null)]
        public string Llf { get; set; }

        [Option("lighting-profile", Required = true, HelpText = "What lighting profile to use. Possible Values: " + nameof(LightingProfile.Distance) + ", " + nameof(LightingProfile.Danae) + ", " + nameof(LightingProfile.DistanceAngle) + ", " + nameof(LightingProfile.DistanceAngleShadow) + ", " + nameof(LightingProfile.DistanceAngleShadowNoTransparency))]
        public LightingProfile LightingProfile
        {
            get; set;
        }

        [Usage(ApplicationAlias = "ArxLibertatisLightingCalculator")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
        new Example("Calculate lighting for level 1 with DistanceAngleShadow",
        new CommandLineArgs { Level = "level1", ArxDataDir = "C:\\Program Files\\Arx Libertatis", LightingProfile = LightingProfile.DistanceAngleShadow })
      };
            }
        }
    }
}
