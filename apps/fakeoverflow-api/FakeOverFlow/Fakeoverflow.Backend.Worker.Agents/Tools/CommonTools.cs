using System.ComponentModel;

namespace Fakeoverflow.Backend.Worker.Agents.Tools;

public static class CommonTools
{
    [Description("Gets the current time in the format yyyy-MM-dd HH:mm:ss")]
    public static string GetCurrentTime() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}