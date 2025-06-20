using ArxLibertatisEditorIO.MediumIO;
using ArxLibertatisEditorIO.RawIO;
using ArxLibertatisLightingCalculatorLib;
using ArxLibertatisLightingCalculatorLib.GI;
using CommandLine.Text;
using System;
using System.IO;
using System.Linq;

namespace ArxLibertatisLightingCalculator
{
    public class Program
    {
        public static void Calculate(string dlf, string llf, string fts, LightingProfile lightingProfile, bool dontCompressFts)
        {
            RawArxLevel ral = new();
            ral.LoadLevel(dlf, llf, fts);
            MediumArxLevel mal = new();
            mal.LoadFrom(ral);

            Console.WriteLine("using " + lightingProfile);
            ArxLibertatisLightingCalculatorLib.ArxLibertatisLightingCalculator.Calculate(mal, lightingProfile);

            mal.SaveTo(ral);
            File.Copy(llf, llf + ".bak", true); //make a backup copy of the llf, just in case
            ral.SaveLevel(dlf, llf, fts, !dontCompressFts);
        }

        public static void Calculate(string level, string arxDataDir, LightingProfile lightingProfile, bool dontCompressFts)
        {
            string dlf = Path.Combine(arxDataDir, "graph", "levels", level, level + ".dlf");
            string llf = Path.Combine(arxDataDir, "graph", "levels", level, level + ".llf");
            string fts = Path.Combine(arxDataDir, "game", "graph", "levels", level, "fast.fts");
            Calculate(dlf, llf, fts, lightingProfile, dontCompressFts);
        }

        static CommandLineArgs cmdLineArgs;
        public static CommandLineArgs CmdLineArgs
        {
            get { return cmdLineArgs; }
        }

        public static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<CommandLineArgs>(args);

            if (args.Length == 0)
            {
                var helpText = HelpText.AutoBuild(result, h => h, e => e);
                Console.WriteLine(helpText);
                return;
            }

            if (result.Errors.Any())
            {
                foreach (var err in result.Errors)
                {
                    Console.WriteLine(err);
                }
                return;
            }
            cmdLineArgs = result.Value;

            if (cmdLineArgs.Level != null)
            {
                Calculate(cmdLineArgs.Level, cmdLineArgs.ArxDataDir, cmdLineArgs.LightingProfile, cmdLineArgs.DontCompressFts);
            }
            else if (cmdLineArgs.Dlf != null)
            {
                Calculate(cmdLineArgs.Dlf, cmdLineArgs.Llf, cmdLineArgs.Fts, cmdLineArgs.LightingProfile, cmdLineArgs.DontCompressFts);
            }
            else
            {
                Console.WriteLine("you have to provide some arguments so this works");
            }
        }
    }
}
