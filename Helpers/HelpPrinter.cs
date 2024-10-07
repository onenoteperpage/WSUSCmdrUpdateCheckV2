using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUSCmdrUpdateCheckV2.Helpers
{
    public static class HelpPrinter
    {
        public static void PrintHelp()
        {
            Console.WriteLine("Usage: WSUSCmdrUpdateCheckV2 --machine=<machine IP or DNS> [--install=<true/false>] [--reboot=<true/false>]");
            Console.WriteLine("    --machine: (required) The IP or DNS of the remote machine.");
            Console.WriteLine("    --install: (optional) Whether to install updates. Defaults to false.");
            Console.WriteLine("    --reboot: (optional) Whether to reboot after installing updates. Requires --install=true.");
            Console.WriteLine("Example: WSUSCmdrUpdateCheckV2 --machine=192.168.1.100 --install=true --reboot=true");
        }
    }
}
