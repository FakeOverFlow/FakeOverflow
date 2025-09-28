using FakeoverFlow.Backend.Abstraction.Errors;

namespace FakeoverFlow.Backend.Http.Api.Errors;

public partial class Errors
{
    /// <summary>
    /// Represents an unknown error that does not fall into predefined categories.
    /// Used as a fallback error type when the specific nature of the error is not determined.
    /// </summary>
    /// <remarks>
    /// The <see cref="UnknownError"/> is typically utilized when encountering unexpected
    /// exceptions or conditions that do not conform to other error types in the system.
    /// </remarks>
    public static readonly Error UnknownError = new Error("UNKNOWN_ERROR",ErrorType.Unknown ,"An unknown error has occurred.");

    /// <summary>
    /// Indicates an unauthorized access attempt or action by a user or system.
    /// Used to represent scenarios where the user lacks the necessary permissions or credentials
    /// to perform the requested operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Unauthorized"/> error is generally employed in cases involving authentication
    /// or authorization failures, such as missing or invalid credentials, or insufficient permissions
    /// to access a resource or execute an action.
    /// </remarks>
    public static readonly Error Unauthorized = new Error("UNAUTHORIZED",ErrorType.Unauthorized ,"The user is not authorized to perform this action.");

    /// <summary>
    /// Represents an error scenario involving an unidentified or unrecognized user.
    /// Typically used when a user's identity cannot be determined or verified during an operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="UnknownUser"/> error is frequently utilized in authentication or identification
    /// processes where the system is unable to associate a user with the provided information,
    /// or when an attempt is made to reference a user that does not exist.
    /// </remarks>
    public static readonly Error UnknownUser = new Error("UNKNOWN_USER", ErrorType.Unauthorized, "Unknown user");
}