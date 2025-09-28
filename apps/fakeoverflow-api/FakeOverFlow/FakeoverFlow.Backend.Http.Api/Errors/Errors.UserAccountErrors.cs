using FakeoverFlow.Backend.Abstraction.Errors;

namespace FakeoverFlow.Backend.Http.Api.Errors;

public partial class Errors
{
    public static class UserAccountErrors
    {
        /// <summary>
        /// Represents an error indicating that a user account could not be found.
        /// </summary>
        /// <remarks>
        /// This error is used when a specific user account does not exist in the system, or it cannot be located
        /// based on the provided information. It is classified under the "NotFound" error type.
        /// </remarks>
        public static readonly Error UserAccountNotFound = new Error("USER_ACCOUNT_NOT_FOUND", ErrorType.NotFound, "The user account was not found.");
    }
}