namespace FakeoverFlow.Backend.Http.Api.Abstracts.Clients;

public interface IStorageClient
{
    /// <summary>
    /// Generates a pre-signed URL for the specified operation.
    /// </summary>
    /// <param name="key">The object key/path</param>
    /// <param name="operation">The HTTP operation (GET or POST)</param>
    /// <param name="expiryMinutes">URL expiry time in minutes. Default: 1 minute for GET, 15 minutes for POST</param>
    /// <param name="contentType">Content type for POST operations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pre-signed URL</returns>
    Task<string> GeneratePreSignedUrlAsync(
        string key, 
        HttpMethod operation, 
        int? expiryMinutes = null,
        string? contentType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an object from storage.
    /// </summary>
    /// <param name="key">The object key/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stream containing the object data, or null if not found</returns>
    Task<Stream?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an object from storage.
    /// </summary>
    /// <param name="key">The object key/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted successfully, false if object didn't exist</returns>
    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an object exists in storage.
    /// </summary>
    /// <param name="key">The object key/path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if object exists, false otherwise</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    
    
}