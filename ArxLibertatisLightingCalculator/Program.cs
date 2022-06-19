using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.RawIO;
using Plexdata.ArgumentParser.Extensions;
using System;
using System.IO;

namespace ArxLibertatisLightingCalculator
{
    public class Program
    {
        public static void Calculate(string dlf, string llf, string fts, LightingProfile lightingProfile)
        {
            RawArxLevel ral = new RawArxLevel();
            ral.LoadLevel(dlf, llf, fts);
            MediumArxLevel mal = new MediumArxLevel();
            mal.LoadLevel(ral);

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
            }
            Console.WriteLine("using " + lightingProfile);
            calculator.Calculate(mal);

            mal.SaveLevel(ral);
            File.Copy(llf, llf + ".bak", true); //make a backup copy of the llf, just in case
            ral.SaveLevel(dlf, llf, fts);
        }

        public static void Calculate(string level, string arxDataDir, LightingProfile lightingProfile)
        {
            string dlf = Path.Combine(arxDataDir, "graph", "levels", level, level + ".dlf");
            string llf = Path.Combine(arxDataDir, "graph", "levels", level, level + ".llf");
            string fts = Path.Combine(arxDataDir, "game", "graph", "levels", level, "fast.fts");
            Calculate(dlf, llf, fts, lightingProfile);
        }

        public static readonly CommandLineArgs cmdLineArgs = new CommandLineArgs();

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(cmdLineArgs.Generate());
                return;
            }
            cmdLineArgs.Process(args);

            if (cmdLineArgs.Level != null)
            {
                Calculate(cmdLineArgs.Level, cmdLineArgs.ArxDataDir, cmdLineArgs.LightingProfile);
            }
            else if (cmdLineArgs.Dlf != null)
            {
                Calculate(cmdLineArgs.Dlf, cmdLineArgs.Llf, cmdLineArgs.Fts, cmdLineArgs.LightingProfile);
            }
            else
            {
                Console.WriteLine("you have to provide some arguments so this works");
            }
        }
    }
}
