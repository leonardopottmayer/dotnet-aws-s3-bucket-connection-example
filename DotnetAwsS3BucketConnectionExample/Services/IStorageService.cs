using DotnetAwsS3BucketConnectionExample.Models;

namespace DotnetAwsS3BucketConnectionExample.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object obj, AwsCredentials awsCredentialsValues);
        Task<List<string>> ListFilesInBucketAsync(AwsCredentials awsCredentialsValues);
        Task<S3Object> GetFileByIdentifierAsync(AwsCredentials awsCredentialsValues, string identifier);
        Task<bool> DeleteFileAsync(AwsCredentials awsCredentialsValues, string key);
    }
}
