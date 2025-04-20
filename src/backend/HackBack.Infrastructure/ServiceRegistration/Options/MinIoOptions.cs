namespace HackBack.Infrastructure.ServiceRegistration.Options
{
    internal class MinIoOptions
    {
        public string AccessKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string BucketName { get; set; } = default!;
    }
}
