using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUSCmdrUpdateCheckV2.Helpers;

public static class HelpPrinter
{
    public static void PrintHelp()
    {
        Console.WriteLine("Usage: WSUSCmdrUpdateCheckV2 <action> [options]");
        Console.WriteLine("Actions:");
        Console.WriteLine("    count         Display the number of updates available.");
        Console.WriteLine("    install       Install available updates.");
        Console.WriteLine("");
        Console.WriteLine("Options:");
        Console.WriteLine("    --machine=<machine IP or DNS>   Specify the machine to check or install updates on. Defaults to the current machine.");
        Console.WriteLine("    --restart                      Automatically restart the machine after installing updates (only with 'install').");
        Console.WriteLine("");
        Console.WriteLine("Examples:");
        Console.WriteLine("    WSUSCmdrUpdateCheckV2 count");
        Console.WriteLine("    WSUSCmdrUpdateCheckV2 count --machine=192.168.1.100");
        Console.WriteLine("    WSUSCmdrUpdateCheckV2 install --machine=192.168.1.100 --restart");
    }
}

