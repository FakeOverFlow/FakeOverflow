namespace FakeoverFlow.Backend.Http.Api.Utils;

public static class TagColorPalette
{
    private static readonly List<string> Colors = new()
    {
        "#A5D8FF", // Soft blue
        "#B2F2BB", // Soft green
        "#FFD6A5", // Peachy orange
        "#FFC9DE", // Light pink
        "#E0C3FC", // Lavender purple
        "#FFF3B0", // Pale yellow
        "#C0EB75", // Lime mint
        "#FFB5A7", // Coral blush
        "#B5EAD7", // Aqua mint
        "#E2ECE9" // Very light grey-green
    };

    private static readonly Random _random = new();

    public static string GetRandomColor()
    {
        int index = _random.Next(Colors.Count);
        return Colors[index];
    }
}