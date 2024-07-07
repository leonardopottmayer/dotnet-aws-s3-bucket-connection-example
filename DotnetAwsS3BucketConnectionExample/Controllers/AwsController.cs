using DotnetAwsS3BucketConnectionExample.Models;
using DotnetAwsS3BucketConnectionExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAwsS3BucketConnectionExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        private readonly ILogger<AwsController> _logger;

        public AwsController(ILogger<AwsController> logger, IConfiguration config, IStorageService storageService)
        {
            _logger = logger;
            _config = config;
            _storageService = storageService;
        }

        [HttpPost("Upload", Name = "UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileExt = Path.GetExtension(file.FileName);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "");

            var docName = $"{Guid.NewGuid().ToString()}-{fileName}{fileExt}";

            var s3Obj = new S3Object()
            {
                BucketName = _config["AwsConfiguration:AWSBucketName"],
                InputStream = memoryStream,
                Name = docName
            };

            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                BucketRegion = _config["AwsConfiguration:AWSBucketRegion"]
            };

            var result = await _storageService.UploadFileAsync(s3Obj, cred);

            return Ok(result);
        }

        [HttpGet("ListFiles", Name = "ListFilesInBucket")]
        public async Task<IActionResult> ListFilesInBucket()
        {
            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                BucketRegion = _config["AwsConfiguration:AWSBucketRegion"],
                BucketName = _config["AwsConfiguration:AWSBucketName"]
            };

            var files = await _storageService.ListFilesInBucketAsync(cred);

            return Ok(files);
        }

        [HttpGet("GetFile/{identifier}", Name = "GetFileByIdentifier")]
        public async Task<IActionResult> GetFileByIdentifier(string identifier)
        {
            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                BucketRegion = _config["AwsConfiguration:AWSBucketRegion"],
                BucketName = _config["AwsConfiguration:AWSBucketName"]
            };

            var file = await _storageService.GetFileByIdentifierAsync(cred, identifier);

            return File(file.InputStream, "application/octet-stream", file.Name);
        }

        [HttpDelete("DeleteFile/{key}", Name = "DeleteFile")]
        public async Task<IActionResult> DeleteFile(string key)
        {
            var cred = new AwsCredentials()
            {
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"],
                BucketRegion = _config["AwsConfiguration:AWSBucketRegion"],
                BucketName = _config["AwsConfiguration:AWSBucketName"]
            };

            var success = await _storageService.DeleteFileAsync(cred, key);

            if (success)
            {
                return Ok("File deleted successfully");
            }
            else
            {
                return BadRequest("Failed to delete file");
            }
        }
    }
}
