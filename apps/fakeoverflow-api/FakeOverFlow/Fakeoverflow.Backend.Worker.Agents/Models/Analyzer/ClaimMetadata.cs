using System.Text;

namespace Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;

public class ClaimMetadata
{
    public List<string> LocationHints { get; set; } = [];
    
    public List<string> DateTimeHints { get; set; } = [];
    
    public List<string> ObjectHints { get; set; } = [];
    
    public List<string> UrlHints { get; set; } = [];
    
    public List<string> PeopleHints { get; set; } = [];

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        
        builder.Append("LocationHints: (");
        builder.Append(string.Join(", ", LocationHints));
        builder.Append(')');
        
        builder.Append(Environment.NewLine);
        
        builder.Append("DateTimeHints: (");
        builder.Append(string.Join(", ", DateTimeHints));
        builder.Append(')');

        builder.Append(Environment.NewLine);
        
        builder.Append("ObjectHints: (");
        builder.Append(string.Join(", ", ObjectHints));
        builder.Append(')');

        builder.Append(Environment.NewLine);
        
        builder.Append("UrlHints: (");
        builder.Append(string.Join(", ", UrlHints));
        builder.Append(')');

        builder.Append(Environment.NewLine);
        
        builder.Append("PeopleHints: (");
        builder.Append(string.Join(", ", PeopleHints));
        builder.Append(')');

        return builder.ToString();
    }
}