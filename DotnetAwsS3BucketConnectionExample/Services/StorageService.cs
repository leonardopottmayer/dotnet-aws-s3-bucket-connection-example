using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DotnetAwsS3BucketConnectionExample.Models;
using System.Net;
using S3Object = DotnetAwsS3BucketConnectionExample.Models.S3Object;

namespace DotnetAwsS3BucketConnectionExample.Services
{
    public class StorageService : IStorageService
    {
        public StorageService() { }

        public async Task<S3ResponseDto> UploadFileAsync(S3Object obj, AwsCredentials awsCredentialsValues)
        {
            Console.WriteLine($"Key: {awsCredentialsValues.AccessKey}, Secret: {awsCredentialsValues.SecretKey}, Region: {awsCredentialsValues.BucketRegion}");

            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsCredentialsValues.BucketRegion)
            };

            var response = new S3ResponseDto();

            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                using var client = new AmazonS3Client(credentials, config);
                var transferUtility = new TransferUtility(client);

                await transferUtility.UploadAsync(uploadRequest);

                response.StatusCode = 201;
                response.Message = $"{obj.Name} has been uploaded successfully";
            }
            catch (AmazonS3Exception s3Ex)
            {
                response.StatusCode = (int)s3Ex.StatusCode;
                response.Message = s3Ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<List<string>> ListFilesInBucketAsync(AwsCredentials awsCredentialsValues)
        {
            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsCredentialsValues.BucketRegion)
            };

            using var client = new AmazonS3Client(credentials, config);

            var response = await client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = awsCredentialsValues.BucketName
            });

            return response.S3Objects.Select(obj => obj.Key).ToList();
        }

        public async Task<S3Object> GetFileByIdentifierAsync(AwsCredentials awsCredentialsValues, string identifier)
        {
            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsCredentialsValues.BucketRegion)
            };

            using var client = new AmazonS3Client(credentials, config);

            var response = await client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = awsCredentialsValues.BucketName,
                Key = identifier
            });

            var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new S3Object
            {
                BucketName = awsCredentialsValues.BucketName,
                Name = identifier,
                InputStream = memoryStream
            };
        }

        public async Task<bool> DeleteFileAsync(AwsCredentials awsCredentialsValues, string key)
        {
            var credentials = new BasicAWSCredentials(awsCredentialsValues.AccessKey, awsCredentialsValues.SecretKey);
            var config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsCredentialsValues.BucketRegion)
            };

            using var client = new AmazonS3Client(credentials, config);

            var response = await client.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = awsCredentialsValues.BucketName,
                Key = key
            });

            return response.HttpStatusCode == HttpStatusCode.NoContent;
        }
    }
}
