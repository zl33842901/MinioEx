using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace xLiAd.MinioEx.AspNetCore
{
    public static class MinioExtension
    {
        public static IServiceCollection AddMinioEx(this IServiceCollection services, Action<IServiceProvider, MinioOptions> action)
        {
            services.AddScoped<IMinioEx>(sp =>
            {
                MinioOptions options = new MinioOptions();
                action?.Invoke(sp, options);
                var result = new MinioEx(options.MinioUrl, options.BucketName, options.AccessKey, options.SecretKey,
                    options.ImageProxyUrl, options.DirectoryPolicy);
                return result;
            });
            return services;
        }
    }
}
