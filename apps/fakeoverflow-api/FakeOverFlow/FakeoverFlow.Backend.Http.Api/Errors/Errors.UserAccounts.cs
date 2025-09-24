using FakeoverFlow.Backend.Abstraction.Errors;

namespace FakeoverFlow.Backend.Http.Api.Errors;

public partial class Errors
{
    public static class UserAccounts
    {
        /// <summary>
        /// Represents an error indicating that the user's account has been disabled.
        /// This error has the identifier "ACCOUNT_DISABLED", belongs to the
        /// <see cref="ErrorType.Unauthorized"/> category, and includes a description
        /// specifying that the account is disabled.
        /// </summary>
        public static readonly Error AccountDisabled =
            new Error("ACCOUNT_DISABLED", ErrorType.Unauthorized, "Account is disabled.");

        /// <summary>
        /// Represents an error indicating that the provided email address is already in use.
        /// This error has the identifier "EMAIL_IN_USE", belongs to the
        /// <see cref="ErrorType.Conflict"/> category, and includes a description
        /// specifying that the email is already in use.
        /// </summary>
        public static readonly Error EmailInUse = new Error("EMAIL_IN_USE", ErrorType.Conflict, "Email is already in use");

        /// <summary>
        /// Represents an error indicating that the chosen username is already in use.
        /// This error has the identifier "USERNAME_IN_USE", belongs to the
        /// <see cref="ErrorType.Conflict"/> category, and includes a description
        /// specifying that the username is already taken.
        /// </summary>
        public static readonly Error UsernameInUse = new Error("USERNAME_IN_USE", ErrorType.Conflict, "Username is already in use");

        /// <summary>
        /// Represents an error indicating that the requested user account was not found.
        /// This error has the identifier "ACCOUNT_NOT_FOUND", belongs to the
        /// <see cref="ErrorType.NotFound"/> category, and includes a description
        /// specifying that the account could not be located.
        /// </summary>
        public static readonly Error NotFound =
            new Error("ACCOUNT_NOT_FOUND", ErrorType.NotFound, "Account not found.");

        /// <summary>
        /// Represents an error indicating that the system failed to create an OAuth account.
        /// This error has the identifier "FAILED_TO_CREATE_OAUTH_ACCOUNT", belongs to the
        /// <see cref="ErrorType.BadRequest"/> category, and includes a description specifying
        /// the failure in creating the OAuth account.
        /// </summary>
        public static readonly Error FailedToCreateOAuthAccount =
            new Error("FAILED_TO_CREATE_OAUTH_ACCOUNT", ErrorType.BadRequest, "Failed to create OAuth account.");

        /// <summary>
        /// Represents an error indicating that the login method used is invalid.
        /// This error has the identifier "INVALID_LOGIN_METHOD", belongs to the
        /// <see cref="ErrorType.BadRequest"/> category, and includes a description
        /// specifying that a valid login method such as Google login is required.
        /// </summary>
        public static readonly Error InvalidLoginMethod =
            new Error("INVALID_LOGIN_METHOD", ErrorType.BadRequest, "Invalid login method. Please use google login");

        /// <summary>
        /// Represents an error indicating that the provided credentials are invalid.
        /// This error has the identifier "INVALID_CREDENTIALS", belongs to the
        /// <see cref="ErrorType.BadRequest"/> category, and includes a description
        /// specifying that the credentials are invalid.
        /// </summary>
        public static readonly Error InvalidCredentials = new Error("INVALID_CREDENTIALS", ErrorType.BadRequest, "Invalid credentials");
    }
    
}