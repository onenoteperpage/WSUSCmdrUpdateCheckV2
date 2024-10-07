namespace WindowsUpdateChecker.Models;

public class UpdateInfo
{
    public string? Title { get; set; }
    public string? UpdateId { get; set; }

    public override string ToString()
    {
        return $"- {Title} (ID: {UpdateId})";
    }
}
