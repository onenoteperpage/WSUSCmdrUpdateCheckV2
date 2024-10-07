using System;
using System.Threading.Tasks;
using WindowsUpdateChecker.Services;
using WSUSCmdrUpdateCheckV2.Helpers;

namespace WSUSCmdrUpdateCheckV2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                HelpPrinter.PrintHelp();
                return;
            }

            // Parse the command line arguments using the helper method
            var options = ArgumentParser.ParseArguments(args);
            if (options == null || string.IsNullOrEmpty(options.Machine))
            {
                HelpPrinter.PrintHelp();
                return;
            }

            string remoteMachine = options.Machine;
            bool install = options.Install;
            bool reboot = options.Reboot;

            IUpdateService updateService = new UpdateService();

            // Check for updates and print the count
            int updateCount = await updateService.CheckForUpdatesAsync(remoteMachine);
            Console.WriteLine($"Total updates found: {updateCount}");

            if (!install)
            {
                if (reboot)
                {
                    Console.WriteLine("Error: Reboot option cannot be used without installing updates.");
                    HelpPrinter.PrintHelp();
                    return;
                }
                // Exit if no installation is requested
                return;
            }

            // Install updates (and possibly reboot)
            await updateService.DownloadAndInstallUpdatesAsync(remoteMachine, install, reboot, "The system will shut down for updates in 3 minutes. Please save your work.");
        }
    }
}
