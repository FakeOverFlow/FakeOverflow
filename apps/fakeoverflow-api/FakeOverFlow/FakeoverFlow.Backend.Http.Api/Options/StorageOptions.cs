using System.ComponentModel.DataAnnotations;

namespace FakeoverFlow.Backend.Http.Api.Options;

public class StorageOptions
{
    public const string SectionName = "Storage";
    
    [Required]
    public string Provider { get; set; } = string.Empty;
    public S3StorageOptions S3 { get; set; } = new();
}


public class S3StorageOptions
{
    [Required]
    public string BucketName { get; set; } = string.Empty;
    
    [Required]
    public string AccessKey { get; set; } = string.Empty;
    
    [Required]
    public string SecretKey { get; set; } = string.Empty;
    
    [Required]
    public string Region { get; set; } = "us-east-1";
    
    public string? ServiceUrl { get; set; }
}