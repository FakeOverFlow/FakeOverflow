using System.Text;
using FakeoverFlow.Backend.Abstraction;
using FakeoverFlow.Backend.Http.Api.Abstracts.Services;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Available;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Login;
using FakeoverFlow.Backend.Http.Api.Features.Auth.Signup;
using FakeoverFlow.Backend.Http.Api.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FakeoverFlow.Backend.Http.Api.Services;

public class UserService(
    AppDbContext dbContext,
    IPasswordHasher<UserAccount> passwordHasher,
    ILogger<UserService> logger
    )
    : IUserService
{
    
    /// <inheritdoc />
    public async Task<Result<UserAccount>> CreateUserAsync(Signup.Request request, bool verifyEmail = false, CancellationToken? cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Trying to create a new user account for {Email} with username {Username}", request.Email,
                request.Username);
            var id = Guid.CreateVersion7();
            var user = new UserAccount()
            {
                Id = id,
                Email = request.Email,
                CreatedBy = id,
                CreatedOn = DateTimeOffset.UtcNow,
                ProfileImageUrl = null,
                IsDisabled = false,
                IsDeleted = false,
                Username = request.Username,
                UpdatedBy = id,
                UpdatedOn = DateTimeOffset.UtcNow,
                VerifiedOn = verifyEmail ? DateTimeOffset.UtcNow : null,
                Settings = new UserAccountSettings()
            };
            var hashedPassword = passwordHasher.HashPassword(user, request.Password);
            var passwordBytes = Encoding.UTF8.GetBytes(hashedPassword);
            user.Password = passwordBytes;

            dbContext.UserAccounts.Add(user);
            await dbContext.SaveChangesAsync();
            return Result<UserAccount>.Success(user);
        }
        catch (Exception e)
        {
            if(e.InnerException is null)
            {
                logger.LogError(e, "Failed to create the user");
                return Result<UserAccount>.Failure(Errors.Errors.UnknownError);
            }

            if (e.InnerException is PostgresException postgresException)
            {
                switch (postgresException.ConstraintName)
                {
                    case "IX_UserAccounts_Email":
                        return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.EmailInUse);
                    case "IX_UserAccounts_Username":
                        return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.UsernameInUse);
                    default:
                        logger.LogError(e, "Failed to create the user");
                        break;
                }
            }
            return Result<UserAccount>.Failure(Errors.Errors.UnknownError);

        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="userId"><inheritdoc/></param>
    /// <param name="cancellationToken"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    public async Task<Result<UserAccountVerification>> GetOrCreateUserVerificationAsync(Guid userId, CancellationToken? cancellationToken = default)
    {
        logger.LogDebug("Trying to get or create a user verification record for user {UserId}", userId);
        var verifications = await dbContext.UserAccountVerifications.Where(x => x.UserId == userId)
            .Include(x => x.Account)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (verifications is null)
            return await CreateUserVerificationAsync(userId, cancellationToken);

        if (verifications.Account.IsDisabled || verifications.Account.IsDeleted)
            return Result<UserAccountVerification>.Failure(Errors.Errors.AuthenticationErrors.AccountDisabled);
        
        return Result<UserAccountVerification>.Success(new UserAccountVerification()
        {
            UserId = userId,
            VerificationToken = verifications.VerificationToken,
        });
    }

    public async Task<Result<bool>> VerifyAccountAsync(string verifyId, CancellationToken? cancellationToken = default)
    {
        var userAccountVerification = await dbContext.UserAccountVerifications
            .Where(x => x.VerificationToken == verifyId)
            .Include(x => x.Account)
            .FirstOrDefaultAsync();
        
        if(userAccountVerification is null)
            return Result<bool>.Failure(Errors.Errors.AuthenticationErrors.InvalidToken);
        
        if(userAccountVerification.Account.VerifiedOn is not null)
            return Result<bool>.Failure(Errors.Errors.AuthenticationErrors.AlreadyVerified);
        
        userAccountVerification.Account.VerifiedOn = DateTimeOffset.UtcNow;
        dbContext.UserAccountVerifications.Remove(userAccountVerification);
        await dbContext.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<UserAccount?> GetUserByIdAsync(Guid userId, bool track = false, CancellationToken? cancellationToken = default)
    {
        var userAccounts = dbContext
            .UserAccounts
            .AsQueryable();

        if (!track)
        {
            logger.LogDebug("Disabling tracking for user {UserId}", userId);
            userAccounts = userAccounts.AsNoTracking();
        }

        var user = await userAccounts.Where(x => x.Id == userId).FirstOrDefaultAsync();
        return user;
    }

    public async Task<Result<UserAccount>> LoginAsync(Login.Request request, CancellationToken? cancellationToken = default)
    {
        var userAccounts = dbContext
            .UserAccounts
            .AsNoTracking();
        
        
        if (request.Type == Login.AuthenticationType.Credentials)
        {
            userAccounts = userAccounts.Where(x => x.Email == request.UserName || x.Username == request.UserName);   
        } else if (request.Type == Login.AuthenticationType.Google)
        {
            //TODO
        }

        var account = await userAccounts.FirstOrDefaultAsync();

        if (account is null)
        {
            // If no account if its username/password login, then stop the exectution
            if(request.Type == Login.AuthenticationType.Credentials)
                return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.NotFound);

            // If no account if its google login, then create a new account
            var userCreationResult = await CreateUserAsync(new Signup.Request()
            {
                Email = string.Empty,
                Password = null,
                Username = request.UserName,
            }, verifyEmail: true, cancellationToken: cancellationToken);
            
            // Stop execution if the user creation failed
            if (!userCreationResult.IsSuccess)
            {
                logger.LogError("Failed to create a new user account for google login due to {Error}", string.Join(", ", userCreationResult.Error!.Description));
                return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.FailedToCreateOAuthAccount);
            }

            return userCreationResult;
        }

        if (account.IsDisabled || account.IsDeleted)
        {
            logger.LogTrace("User {UserId} is disabled", account.Id);
            return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.AccountDisabled);
        }

        if (account.VerifiedOn is null)
        {
            logger.LogTrace("User {UserId} is not verified", account.Id);
            return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.AccountNotVerified);
        } 

        if (account.Password is null)
        {
            logger.LogWarning("User is signed up using GoogleOAuth");
            return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.InvalidLoginMethod);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning("Password is empty");
            return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.InvalidLoginMethod);
        }

        var passwordString = Encoding.UTF8.GetString(account.Password);
        var isPasswordValid = passwordHasher.VerifyHashedPassword(account, passwordString, request.Password) == PasswordVerificationResult.Success;
        
        if(isPasswordValid)
            return Result<UserAccount>.Success(account);
        
        //TODO Update last invalid login date and attempt count and maybe block the account for a while
        // dbContext.UserAccounts.FromSqlInterpolated(
        //     "UPDATE UserAccounts SET LastInvalidLoginDate = {now}, AttemptCount = AttemptCount + 1 WHERE Id = {userId}");
        
        return Result<UserAccount>.Failure(Errors.Errors.AuthenticationErrors.InvalidCredentials);
    }

    private async Task<Result<UserAccountVerification>> CreateUserVerificationAsync(Guid userId, CancellationToken? cancellationToken = default)
    {
        logger.LogDebug("Trying to create a user verification record for user {UserId}", userId);
        try
        {
            var verification = new UserAccountVerification()
            {
                VerificationToken = Ulid.NewUlid().ToString(),
                UserId = userId,
                CreatedOn = DateTimeOffset.UtcNow,
            };
            dbContext.UserAccountVerifications.Add(verification);
            logger.LogDebug("User verification record created for user {UserId}, {VerificationToken}", userId,
                verification.VerificationToken);
            await dbContext.SaveChangesAsync();
            
            verification.Account = await GetUserByIdAsync(userId, cancellationToken: cancellationToken) ?? throw new Exception("User not found");
            return Result<UserAccountVerification>.Success(verification);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating user verification record for user {UserId}", userId);
            return Result<UserAccountVerification>.Failure(Errors.Errors.UnknownError);
        }
    }

    public async Task<Result<RefreshTokens>> CreateRefreshTokenForUserAsync(Guid userId, string jti, CancellationToken? cancellationToken = default)
    {
        logger.LogDebug("Trying to create a refresh token for user {UserId}", userId);
        try
        {
            var token = new RefreshTokens()
            {
                Id = Ulid.NewUlid().ToString(),
                JTI = jti,
                UserId = userId,
                CreatedOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(10),
                RevokedOn = null,
            };
            
            dbContext.RefreshTokens.Add(token);
            await dbContext.SaveChangesAsync();
            return Result<RefreshTokens>.Success(token);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating refresh token for user {UserId}", userId);
            return Result<RefreshTokens>.Failure(Errors.Errors.UnknownError);
        }
    }
    

    public async Task DeleteRefreshTokenAsync(Guid userId, string jti, CancellationToken? cancellationToken = default)
    {
        try
        {
            await dbContext.RefreshTokens.Where(x => x.UserId == userId && x.JTI == jti).ExecuteDeleteAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting refresh token for user {UserId}", userId);
        }
    }

    public async Task<Result<RefreshTokens>> GetRefreshTokenOf(Guid userId, string jti, bool track = false, bool checkExpiry = true, bool checkRevoked = true,
        bool revokeOnCall = false, CancellationToken? cancellationToken = default)
    {
        try
        {
            var queryable = dbContext.RefreshTokens.AsQueryable();
            if (!track || revokeOnCall)
            {
                queryable = queryable.AsNoTracking();
            }

            if (checkExpiry)
            {
                queryable = queryable.Where(x => x.ExpiresOn > DateTimeOffset.UtcNow);
            }

            if (checkRevoked)
            {
                queryable = queryable.Where(x => x.RevokedOn == null);
            }
            
            queryable = queryable.Include(x => x.Account);
            var token = queryable.FirstOrDefault(x => x.UserId == userId && x.JTI == jti);
            
            if(token is null)
                return Result<RefreshTokens>.Failure(Errors.Errors.AuthenticationErrors.InvalidToken);

            if (revokeOnCall)
            {
                await dbContext.RefreshTokens.Where(x => x.Id == token.Id).ExecuteDeleteAsync();
            }
            
            return Result<RefreshTokens>.Success(token);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting refresh token for user {UserId}", userId);
            return Result<RefreshTokens>.Failure(Errors.Errors.UnknownError);
        }
    }

    public async Task<Result<bool>> CheckExistsAsync(Available.Request request, CancellationToken? cancellationToken = default)
    {
        try
        {
            var userAccounts = dbContext.UserAccounts.AsQueryable();
            userAccounts = userAccounts.AsNoTracking();

            switch (request.Type)
            {
                case Available.AvailabilityType.Username:
                    userAccounts = userAccounts.Where(x => x.Username == request.Value);
                    break;
                case Available.AvailabilityType.Email:
                    userAccounts = userAccounts.Where(x => x.Email == request.Value);
                    break;
            }

            var exists = await userAccounts.AnyAsync();
            return Result<bool>.Success(exists);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking if user exists");
            return Result<bool>.Failure(Errors.Errors.UnknownError);;
        }
    }
}