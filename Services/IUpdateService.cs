using System.Threading.Tasks;

namespace WindowsUpdateChecker.Services;

public interface IUpdateService
{
    Task<int> CheckForUpdatesAsync(string machineName);
    Task DownloadAndInstallUpdatesAsync(string machineName, bool install, bool reboot, string message = "The system will shut down for updates in 3 minutes.");
}
