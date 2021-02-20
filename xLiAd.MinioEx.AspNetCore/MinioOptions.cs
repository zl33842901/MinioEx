using System;

namespace xLiAd.MinioEx.AspNetCore
{
    public class MinioOptions
    {
        public string MinioUrl { get; set; }

        public string BucketName { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string ImageProxyUrl { get; set; }

        public DirectoryPolicy DirectoryPolicy { get; set; } = DirectoryPolicy.None;
    }
}
