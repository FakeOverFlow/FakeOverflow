namespace Fakeoverflow.Backend.Worker.Agents.Models.Analyzer;

public class Claim
{
    /// <summary>
    /// Gets or sets the textual representation of a claim.
    /// </summary>
    /// <remarks>
    /// This property holds the main content or statement of the claim as a string.
    /// </remarks>
    public string ClaimText { get; set; } = null!;

    /// <summary>
    /// Gets or sets the category of the claim.
    /// </summary>
    /// <remarks>
    /// This property specifies the type or classification of the claim, determining its nature such as factual or opinion-based.
    /// </remarks>
    public ClaimType Type { get; set; }

    /// <summary>
    /// Represents the type or category of a claim.
    /// </summary>
    /// <remarks>
    /// The enumeration defines various classifications that a claim can belong to,
    /// such as factual, opinion, statistical, historical, or scientific.
    /// </remarks>
    public enum ClaimType
    {
        Factual,
        Opinion,
        Statistical,
        Historical,
        Scientific
    }

    public override string ToString()
    {
        return $"ClaimType: {Type.ToString()} {Environment.NewLine} Claim: {ClaimText}";
    }
}