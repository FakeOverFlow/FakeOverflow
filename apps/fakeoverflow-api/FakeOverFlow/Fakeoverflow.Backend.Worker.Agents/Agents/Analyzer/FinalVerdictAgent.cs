using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Agents.Analyzer;

public class FinalVerdictAgent
{
    public const string ModelName = Constants.Models.BaseModel;

    public const string Instructions = """

                                       """;

    public static readonly ChatClientAgentOptions Options = new ChatClientAgentOptions()
    {
        Instructions = Instructions,
        Description =
            "The agent responsible to access and generate a final verdict about the claims that has been made in a post",
        Name = nameof(FinalVerdictAgent),
        ChatOptions = new ChatOptions()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(AIJsonUtilities.CreateJsonSchema(typeof(FinalVerdictAgentResponse))),
        }
    };
    
    public class FinalVerdictAgentResponse
    {
        public FinalVerdict Verdict { get; set; }
        
        [Description("Score in 0-100")]
        public int Score { get; set; }
        
        public string Reasoning { get; set; } = string.Empty;
        
        public List<string> Contradictions { get; set; } = [];
        
        public List<string> Citations { get; set; } = [];
    }

}

public enum FinalVerdict
{
    Fact,
    LikelyFact,
    UnlikelyFact,
    Rumor,
    Joke,
    False,
    Inconclusive,
}