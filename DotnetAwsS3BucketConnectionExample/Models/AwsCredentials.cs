namespace DotnetAwsS3BucketConnectionExample.Models
{
    public class AwsCredentials
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketRegion { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }
}
