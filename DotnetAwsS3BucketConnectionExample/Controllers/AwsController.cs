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
            var docName = $"{Guid.NewGuid().ToString()}-{file.FileName}.{fileExt}";

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
    }
}
