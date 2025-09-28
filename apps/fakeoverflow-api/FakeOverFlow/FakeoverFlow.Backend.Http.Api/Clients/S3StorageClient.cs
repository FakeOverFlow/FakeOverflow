using Amazon.S3;
using Amazon.S3.Model;
using FakeoverFlow.Backend.Http.Api.Abstracts.Clients;
using FakeoverFlow.Backend.Http.Api.Options;
using Microsoft.Extensions.Options;

namespace FakeoverFlow.Backend.Http.Api.Clients;

public class S3StorageClient : IStorageClient, IDisposable
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageOptions _options;
    private bool _disposed;

    public S3StorageClient(IOptions<StorageOptions> options)
    {
        _options = options.Value.S3;
        
        var config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_options.Region)
        };

        // Support for MinIO or other S3-compatible services
        if (!string.IsNullOrEmpty(_options.ServiceUrl))
        {
            config.ServiceURL = _options.ServiceUrl;
            config.ForcePathStyle = true; // Required for MinIO
        }

        _s3Client = new AmazonS3Client(_options.AccessKey, _options.SecretKey, config);
    }

    public async Task<string> GeneratePreSignedUrlAsync(
        string key, 
        HttpMethod operation, 
        int? expiryMinutes = null,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        ValidateKey(key);
        ValidateOperation(operation);
        ThrowIfDisposed();

        var defaultExpiry = operation.Method == "GET" ? 1 : 15;
        var expiry = TimeSpan.FromMinutes(expiryMinutes ?? defaultExpiry);

        if (operation.Method == "GET")
        {
            var getRequest = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(expiry)
            };

            return await Task.FromResult(_s3Client.GetPreSignedURL(getRequest));
        }
        else // POST
        {
            var postRequest = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Verb = HttpVerb.PUT, // S3 uses PUT for uploads via pre-signed URLs
                Expires = DateTime.UtcNow.Add(expiry)
            };

            if (!string.IsNullOrEmpty(contentType))
            {
                postRequest.ContentType = contentType;
            }

            return await _s3Client.GetPreSignedURLAsync(postRequest);
        }
    }

    public async Task<Stream?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        ValidateKey(key);
        ThrowIfDisposed();

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        ValidateKey(key);
        ThrowIfDisposed();

        try
        {
            // Check if object exists first
            var existsRequest = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(existsRequest, cancellationToken);

            // Object exists, delete it
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ValidateKey(key);
        ThrowIfDisposed();

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _options.BucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates the S3 bucket if it doesn't exist (useful for development/testing).
    /// </summary>
    public async Task EnsureBucketExistsAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        try
        {
            await _s3Client.GetBucketLocationAsync(_options.BucketName, cancellationToken);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var request = new PutBucketRequest
            {
                BucketName = _options.BucketName,
                UseClientRegion = true
            };

            await _s3Client.PutBucketAsync(request, cancellationToken);
        }
    }

    private static void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        if (key.Length > 1024)
            throw new ArgumentException("Key cannot exceed 1024 characters.", nameof(key));

        // S3 key validation
        if (key.StartsWith('/') || key.EndsWith('/'))
            throw new ArgumentException("Key cannot start or end with '/'.", nameof(key));
    }

    private static void ValidateOperation(HttpMethod operation)
    {
        if (operation.Method != "GET" && operation.Method != "POST" && operation.Method != "PUT")
            throw new ArgumentException("Only GET and POST operations are supported.", nameof(operation));
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _s3Client?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}