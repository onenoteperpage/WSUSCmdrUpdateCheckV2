using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUSCmdrUpdateCheckV2.Helpers
{
    public class ArgumentParser
    {
        public string Machine { get; set; } = null!;
        public bool Install { get; set; } = false;
        public bool Reboot { get; set; } = false;

        public static ArgumentParser? ParseArguments(string[] args)
        {
            var options = new ArgumentParser();

            foreach (var arg in args)
            {
                if (arg.StartsWith("--machine="))
                {
                    options.Machine = arg.Split('=')[1];
                }
                else if (arg.StartsWith("--install="))
                {
                    options.Install = bool.TryParse(arg.Split('=')[1], out bool installValue) && installValue;
                }
                else if (arg.StartsWith("--reboot="))
                {
                    options.Reboot = bool.TryParse(arg.Split('=')[1], out bool rebootValue) && rebootValue;
                }
                else
                {
                    Console.WriteLine($"Unknown argument: {arg}");
                    return null;
                }
            }

            return options;
        }
    }
}
