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
            if (options == null)
            {
                HelpPrinter.PrintHelp();
                return;
            }

            string remoteMachine = options.Machine ?? Environment.MachineName; // Use current machine if none provided
            bool install = options.Install;
            bool reboot = options.Reboot;

            IUpdateService updateService = new UpdateService();

            // Execute based on the action type: count or install
            if (options.Action == "count")
            {
                await CountUpdatesAsync(updateService, remoteMachine);
            }
            else if (options.Action == "install")
            {
                await InstallUpdatesAsync(updateService, remoteMachine, install, reboot);
            }
            else
            {
                Console.WriteLine("Unknown action. Please provide 'count' or 'install'.");
                HelpPrinter.PrintHelp();
            }
        }

        static async Task CountUpdatesAsync(IUpdateService updateService, string machineName)
        {
            int updateCount = await updateService.CheckForUpdatesAsync(machineName);
            Console.WriteLine($"Total updates found: {updateCount}");
        }

        static async Task InstallUpdatesAsync(IUpdateService updateService, string machineName, bool install, bool reboot)
        {
            // Check for updates and print the count first
            int updateCount = await updateService.CheckForUpdatesAsync(machineName);
            Console.WriteLine($"Total updates found: {updateCount}");

            if (install)
            {
                // Download and install updates
                await updateService.DownloadAndInstallUpdatesAsync(machineName, install, reboot, "The system will shut down for updates in 3 minutes. Please save your work.");
            }
            else if (reboot)
            {
                Console.WriteLine("Error: Reboot option cannot be used without installing updates.");
                HelpPrinter.PrintHelp();
            }
        }
    }
}
