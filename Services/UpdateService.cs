using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WUApiLib;
using WindowsUpdateChecker.Models;

namespace WindowsUpdateChecker.Services
{
    public class UpdateService : IUpdateService
    {
        public async Task<int> CheckForUpdatesAsync(string machineName)
        {
            return await Task.Run(() =>
            {
                var updateSession = new UpdateSession();
                var updateSearcher = updateSession.CreateUpdateSearcher();
                updateSearcher.Online = true;

                try
                {
                    // Search for updates
                    ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And Type='Software'");
                    Console.WriteLine($"Found {searchResult.Updates.Count} updates on {machineName}:");

                    foreach (IUpdate update in searchResult.Updates)
                    {
                        Console.WriteLine($"- {update.Title} (Installed: {update.IsInstalled})");
                    }
                    return searchResult.Updates.Count; // Return the count of available updates
                }
                catch (COMException ex)
                {
                    Console.WriteLine($"Failed to check updates on {machineName}: {ex.Message}");
                    return 0; // Indicate failure
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    return 0; // Indicate failure
                }
            });
        }

        public async Task DownloadAndInstallUpdatesAsync(string machineName, bool install, bool reboot, string message = "The system will shut down for updates in 3 minutes.")
        {
            await Task.Run(() =>
            {
                var updateSession = new UpdateSession();
                var updateSearcher = updateSession.CreateUpdateSearcher();
                var updateDownloader = updateSession.CreateUpdateDownloader();
                var updateInstaller = updateSession.CreateUpdateInstaller();
                var updatesToInstall = new UpdateCollection();

                // Explicitly initialize the downloader's updates collection
                updateDownloader.Updates = new UpdateCollection();

                updateSearcher.Online = true;

                try
                {
                    ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And Type='Software'");

                    foreach (IUpdate update in searchResult.Updates)
                    {
                        // Output update info
                        Console.WriteLine($"Title: {update.Title}");
                        Console.WriteLine($"IsInstalled: {update.IsInstalled}");
                        Console.WriteLine($"Downloaded: {update.IsDownloaded}");
                        Console.WriteLine($"IsMandatory: {update.IsMandatory}");

                        // Download the update if not already downloaded
                        if (!update.IsDownloaded)
                        {
                            updateDownloader.Updates.Add(update);
                            var downloadResult = updateDownloader.Download();

                            // Check if the download was successful
                            if (downloadResult.ResultCode == OperationResultCode.orcSucceeded)
                            {
                                Console.WriteLine($"Downloaded: {update.Title}");
                                updatesToInstall.Add(update);
                            }
                            else
                            {
                                Console.WriteLine($"Failed to download: {update.Title}");
                            }
                        }
                        else
                        {
                            updatesToInstall.Add(update);
                        }
                    }

                    // Optionally, handle install
                    if (install)
                    {
                        if (updatesToInstall.Count > 0)
                        {
                            updateInstaller.Updates = updatesToInstall;

                            // Try casting to IUpdateInstaller2 for silent install
                            IUpdateInstaller2? updateInstaller2 = updateInstaller as IUpdateInstaller2;
                            if (updateInstaller2 != null)
                            {
                                updateInstaller2.IsForced = true; // Force a silent install
                            }

                            IInstallationResult installationResult = updateInstaller.Install();

                            for (int i = 0; i < updatesToInstall.Count; i++)
                            {
                                Console.WriteLine($"Installing {updatesToInstall[i].Title}...");
                                if (installationResult.GetUpdateResult(i).HResult == 0)
                                {
                                    Console.WriteLine($"INSTALL SUCCESS FOR {updatesToInstall[i].Title}");
                                }
                                else
                                {
                                    Console.WriteLine($"INSTALL FAIL FOR {updatesToInstall[i].Title}");
                                }
                            }
                        }

                        // Optionally, handle reboot
                        if (reboot)
                        {
                            Console.WriteLine("A reboot is required. The system will shut down in 3 minutes...");
                            string shutdownCommand = $"/r /t 180 /c \"{message}\"";
                            System.Diagnostics.Process.Start("shutdown", shutdownCommand);
                        }
                    }
                }
                catch (COMException ex)
                {
                    Console.WriteLine($"Failed to download and install updates on {machineName}: {ex.Message}");
                }
                catch (NullReferenceException ex)
                {
                    Console.WriteLine($"Null reference encountered: {ex.Message}. Please check object initialization.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
            });
        }


        //public async Task DownloadAndInstallUpdatesAsync(string machineName, bool install, bool reboot, string message = "The system will shut down for updates in 3 minutes.")
        //{
        //    await Task.Run(() =>
        //    {
        //        var updateSession = new UpdateSession();
        //        var updateSearcher = updateSession.CreateUpdateSearcher();
        //        var updateDownloader = updateSession.CreateUpdateDownloader();
        //        var updateInstaller = updateSession.CreateUpdateInstaller();
        //        var updatesToInstall = new UpdateCollection();

        //        // Explicitly initialize the downloader's updates collection
        //        updateDownloader.Updates = new UpdateCollection(); // Fix: initialize the collection

        //        updateSearcher.Online = true;

        //        try
        //        {
        //            ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And Type='Software'");

        //            foreach (IUpdate update in searchResult.Updates)
        //            {
        //                // Output update info
        //                Console.WriteLine($"Title: {update.Title}");
        //                Console.WriteLine($"IsInstalled: {update.IsInstalled}");
        //                Console.WriteLine($"Downloaded: {update.IsDownloaded}");
        //                Console.WriteLine($"IsMandatory: {update.IsMandatory}");

        //                // Download the update if not already downloaded
        //                if (!update.IsDownloaded)
        //                {
        //                    updateDownloader.Updates.Add(update); // Now this works because we initialized the collection
        //                    var downloadResult = updateDownloader.Download();

        //                    // Check if the download was successful
        //                    if (downloadResult.ResultCode == OperationResultCode.orcSucceeded)
        //                    {
        //                        Console.WriteLine($"Downloaded: {update.Title}");
        //                        updatesToInstall.Add(update);
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Failed to download: {update.Title}");
        //                    }
        //                }
        //                else
        //                {
        //                    updatesToInstall.Add(update);
        //                }
        //            }

        //            // Optionally, handle install
        //            if (install)
        //            {
        //                // Install the updates if any are available
        //                if (updatesToInstall.Count > 0)
        //                {
        //                    updateInstaller.Updates = updatesToInstall;
        //                    IInstallationResult installationResult = updateInstaller.Install();

        //                    for (int i = 0; i < updatesToInstall.Count; i++)
        //                    {
        //                        Console.WriteLine($"Installing {updatesToInstall[i].Title}...");
        //                        if (installationResult.GetUpdateResult(i).HResult == 0)
        //                        {
        //                            Console.WriteLine($"INSTALL SUCCESS FOR {updatesToInstall[i].Title}");
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine($"INSTALL FAIL FOR {updatesToInstall[i].Title}");
        //                        }
        //                    }
        //                }

        //                // Optionally, handle reboot
        //                if (reboot)
        //                {
        //                    Console.WriteLine("A reboot is required. The system will shut down in 3 minutes...");
        //                    string shutdownCommand = $"/r /t 180 /c \"{message}\"";
        //                    System.Diagnostics.Process.Start("shutdown", shutdownCommand);
        //                }
        //            }
        //        }
        //        catch (COMException ex)
        //        {
        //            Console.WriteLine($"Failed to download and install updates on {machineName}: {ex.Message}");
        //        }
        //        catch (NullReferenceException ex)
        //        {
        //            Console.WriteLine($"Null reference encountered: {ex.Message}. Please check object initialization.");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        //        }
        //    });
        //}
    }
}
