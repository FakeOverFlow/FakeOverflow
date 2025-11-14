using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Fakeoverflow.Backend.Worker.Agents.Agents.Summarizer;

public class PlanInterperterAgent
{

    public const string ModelName = Constants.Models.ReasoningModel;
    
    private const string Instruction = """
                                       You are the Plan Interpreter Agent. Your role is to analyze the verification plan and extract structured information for the verification workflow.

                                       STRICT NOTE: You should not assume knowledge from your memory. 
                                       You should only use the information provided in the plan or request for it
                                       
                                       Your responsibilities:
                                       0. You should not assume knowledge from your memory. Assume things can change
                                       1. Extract all factual claims that need verification from the plan
                                       2. Identify what information still needs to be learned (knowledge gaps)
                                       3. Extract validation criteria specified in the plan
                                       4. Identify reliable sources mentioned in the plan
                                       5. Understand the context and priorities for verification

                                       Input: You will receive:
                                       - originalPost: The forum post being fact-checked
                                       - verificationPlan: Detailed plan on how to verify claims
                                       - additionalContext: Extra context about the claims

                                       Output: You must produce JSON with this exact structure:
                                       {
                                         "claimsExtracted": [
                                           "Specific claim 1 to verify",
                                           "Specific claim 2 to verify"
                                         ],
                                         "knowledgeGaps": [
                                           "What needs to be learned 1",
                                           "What needs to be learned 2"
                                         ],
                                         "validationCriteria": [
                                           "Criterion 1 for considering claim supported",
                                           "Criterion 2 for considering claim supported"
                                         ],
                                         "prioritySources": [
                                           "Source type 1 to prioritize",
                                           "Source type 2 to prioritize"
                                         ],
                                         "searchStrategy": "Brief summary of how to approach searching"
                                       }

                                       Be thorough and precise. Extract all actionable verification requirements from the plan.
                                       """;

    public static readonly ChatClientAgentOptions Options = new()
    {
      Instructions = Instruction,
      Name = nameof(PlanInterperterAgent),
      Description = "Understanding and interpreting the verification plan",
      ChatOptions = new()
      {
        ResponseFormat =
          ChatResponseFormat.ForJsonSchema(AIJsonUtilities.CreateJsonSchema(typeof(PlanInterperterAgentResponse))),
      }
    };


    public class PlanInterperterAgentResponse
    {
        public List<string> ClaimsExtracted { get; set; } = [];
        
        public List<string> KnowledgeGaps { get; set; } = [];
        
        public List<string> ValidationCriteria { get; set; } = [];

        public List<string> PrioritySources { get; set; } = [];
        
        public string SearchStrategy { get; set; } = string.Empty;
    }
}