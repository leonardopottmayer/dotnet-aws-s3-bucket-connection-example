using DotnetAwsS3BucketConnectionExample.Models;

namespace DotnetAwsS3BucketConnectionExample.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object obj, AwsCredentials awsCredentialsValues);
    }
}
