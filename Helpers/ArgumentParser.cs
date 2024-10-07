namespace WSUSCmdrUpdateCheckV2.Helpers;

public class ArgumentParser
{
    public string? Machine { get; set; }
    public string Action { get; set; } = "count"; // Default action is "count"
    public bool Install { get; set; } = false;
    public bool Reboot { get; set; } = false;

    public static ArgumentParser? ParseArguments(string[] args)
    {
        var options = new ArgumentParser();

        foreach (var arg in args)
        {
            if (arg.StartsWith("--machine=") || arg.StartsWith("--computer="))
            {
                options.Machine = arg.Split('=')[1];
            }
            else if (arg == "install")
            {
                options.Action = "install";
                options.Install = true; // If 'install' is provided, we assume installation
            }
            else if (arg == "count")
            {
                options.Action = "count";
            }
            else if (arg == "--restart" || arg == "--reboot")
            {
                options.Reboot = true;
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
