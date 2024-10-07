using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WUApiLib;
using WindowsUpdateChecker.Models;

namespace WindowsUpdateChecker.Services;

public class UpdateService : IUpdateService
{
    public async Task<int> CheckForUpdatesAsync(string machineName)
    {
        // Logic for checking updates
        return await Task.Run(() =>
        {
            var updateSession = new UpdateSession();
            var updateSearcher = updateSession.CreateUpdateSearcher();
            updateSearcher.Online = true;
            ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And Type='Software'");
            Console.WriteLine($"Found {searchResult.Updates.Count} updates on {machineName}:");
            foreach (IUpdate update in searchResult.Updates)
            {
                Console.WriteLine($"- {update.Title} (Installed: {update.IsInstalled})");
            }
            return searchResult.Updates.Count;
        });
    }

    public async Task DownloadAndInstallUpdatesAsync(string machineName, bool install, bool reboot, string message)
    {
        await Task.Run(() =>
        {
            var updateSession = new UpdateSession();
            var updateSearcher = updateSession.CreateUpdateSearcher();
            var updateDownloader = updateSession.CreateUpdateDownloader();
            var updateInstaller = updateSession.CreateUpdateInstaller();
            var updatesToInstall = new UpdateCollection();

            updateDownloader.Updates = new UpdateCollection();
            updateSearcher.Online = true;

            try
            {
                ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And Type='Software'");

                foreach (IUpdate update in searchResult.Updates)
                {
                    Console.WriteLine($"Title: {update.Title}");
                    if (!update.IsDownloaded)
                    {
                        updateDownloader.Updates.Add(update);
                        var downloadResult = updateDownloader.Download();
                        if (downloadResult.ResultCode == OperationResultCode.orcSucceeded)
                        {
                            updatesToInstall.Add(update);
                        }
                    }
                    else
                    {
                        updatesToInstall.Add(update);
                    }
                }

                if (install && updatesToInstall.Count > 0)
                {
                    updateInstaller.Updates = updatesToInstall;
                    IInstallationResult installationResult = updateInstaller.Install();

                    foreach (IUpdate update in updatesToInstall)
                    {
                        Console.WriteLine($"INSTALL SUCCESS FOR {update.Title}");
                    }
                }

                if (reboot)
                {
                    Console.WriteLine("Rebooting the system...");
                    System.Diagnostics.Process.Start("shutdown", "/r /t 180 /c \"" + message + "\"");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during installation: {ex.Message}");
            }
        });
    }
}
