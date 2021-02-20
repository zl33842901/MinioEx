using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.MinioEx.AspNetCore
{
    public static class MinioExtension
    {
        public static IServiceCollection AddMinioEx(this IServiceCollection services, Action<MinioOptions> action)
        {
            MinioOptions options = new MinioOptions();
            action?.Invoke(options);
            services.AddSingleton<IMinioEx>(sp => new MinioEx(options.MinioUrl, options.BucketName, options.AccessKey, options.SecretKey,
                options.ImageProxyUrl, options.DirectoryPolicy));
            return services;
        }
    }
}
