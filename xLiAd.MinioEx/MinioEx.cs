using System;
using Minio;
using System.IO;
using System.Threading.Tasks;

namespace xLiAd.MinioEx
{
    public class MinioEx : IMinioEx
    {
        private readonly string accessKey, secretKey;
        private readonly string domainAndPort, bucketName, imageProxyUrl;
        //private readonly IEnumerable<string> buildInBucket = new string[] { "pointapp", "workflow", "oa", "crm" };
        private readonly MinioClient minio;
        private readonly DirectoryPolicy directoryPolicy;
        /// <summary>
        /// 初始化 MinIO 对象
        /// </summary>
        /// <param name="domainAndPort">MinIO 地址，如： 172.16.101.28:9000</param>
        /// <param name="bucketName">存储桶名，若不存在，找佃超或张磊开通。</param>
        /// <param name="accessKey">用户名</param>
        /// <param name="secretKey">密码</param>
        /// <param name="imageProxyUrl">ImageProxy 服务地址，如： http://172.16.101.28:8180 没有可以传 null</param>
        /// <param name="directoryPolicy">文件夹规则，每月/每年/每天 一个文件夹，或 不区分</param>
        public MinioEx(string domainAndPort, string bucketName, string accessKey, string secretKey, string imageProxyUrl, DirectoryPolicy directoryPolicy = DirectoryPolicy.None)
        {
            this.domainAndPort = domainAndPort;
            this.bucketName = bucketName;
            this.imageProxyUrl = imageProxyUrl;
            this.directoryPolicy = directoryPolicy;
            this.accessKey = accessKey;
            this.secretKey = secretKey;

            try
            {
                minio = new MinioClient(this.domainAndPort, this.accessKey, this.secretKey);
            }
            catch (Exception e)
            {
                throw new Exception($"初始化 MinIO 客户端时发生错误：{e.Message}", e);
            }
        }

        private string GetSavePath()
        {
            switch (directoryPolicy)
            {
                case DirectoryPolicy.ByMonth:
                    return DateTime.Now.ToString("yyyy-MM") + "/";
                case DirectoryPolicy.ByYear:
                    return DateTime.Now.Year.ToString() + "/";
                case DirectoryPolicy.ByDay:
                    return DateTime.Now.ToString("yyyy-MM-dd") + "/";
                default:
                    return string.Empty;
            }
        }

        private string GetRandomName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid();
        }

        private string ConvertImageContentType(string contentType, string name)
        {
            if (!string.IsNullOrWhiteSpace(contentType))
                return contentType;
            if (name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                return "image/jpeg";
            else if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                return "image/png";
            else if (name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                return "image/gif";
            else
                return null;
        }

        public async Task<string[]> UploadWithNameAsync(byte[] fileContent, string name, string contentType = null)
        {
            if (fileContent == null || fileContent.Length < 1)
                throw new ArgumentNullException("fileContent");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            MemoryStream ms = new MemoryStream(fileContent);
            return await UploadWithNameAsync(ms, name, contentType);
        }

        public async Task<string[]> UploadWithNameAsync(Stream fileContent, string name, string contentType = null)
        {
            if (fileContent == null || fileContent.Length < 1)
                throw new ArgumentNullException("fileContent");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");
            name = GetSavePath() + name;
            try
            {
                await PrepareBucket();
                await minio.PutObjectAsync(bucketName, name, fileContent, fileContent.Length, ConvertImageContentType(contentType, name));
                return GetResult(name);
            }
            catch (Minio.Exceptions.MinioException e)
            {
                throw new Exception($"上传到 MinIO 时遇到问题：{e.Message}", e);
            }
            catch (Exception e)
            {
                throw new Exception($"上传到 MinIO 时遇到问题：{e.Message}", e);
            }
        }
        public async Task<string[]> UploadAsync(byte[] fileContent, string fileExt, string contentType = null)
        {
            if (fileContent == null || fileContent.Length < 1)
                throw new ArgumentNullException("fileContent");
            if (string.IsNullOrWhiteSpace(fileExt))
                throw new ArgumentNullException("fileExt");
            MemoryStream ms = new MemoryStream(fileContent);
            return await UploadAsync(ms, fileExt, contentType);
        }

        public async Task<string[]> UploadAsync(Stream fileContent, string fileExt, string contentType = null)
        {
            if (fileContent == null || fileContent.Length < 1)
                throw new ArgumentNullException("fileContent");
            if (string.IsNullOrWhiteSpace(fileExt))
                throw new ArgumentNullException("fileExt");
            var name = GetRandomName() + "." + fileExt.TrimStart('.');
            name = GetSavePath() + name;
            try
            {
                await PrepareBucket();
                await minio.PutObjectAsync(bucketName, name, fileContent, fileContent.Length, ConvertImageContentType(contentType, name));
                return GetResult(name);
            }
            catch (Minio.Exceptions.MinioException e)
            {
                throw new Exception($"上传到 MinIO 时遇到问题：{e.Message}", e);
            }
            catch (Exception e)
            {
                throw new Exception($"上传到 MinIO 时遇到问题：{e.Message}", e);
            }
        }

        private async Task PrepareBucket()
        {
            //var location = "us-east-1";
            bool found = await minio.BucketExistsAsync(bucketName);
            if (!found)
            {
                //if (buildInBucket.Contains(bucketName))
                //{
                //    await minio.MakeBucketAsync(bucketName, location);
                //    await minio.SetPolicyAsync(bucketName, "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Effect\":\"Allow\",\"Principal\":{\"AWS\":[\"*\"]},\"Action\":[\"s3:GetBucketLocation\",\"s3:ListBucket\"],\"Resource\":[\"arn:aws:s3:::" + bucketName + "\"]},{\"Effect\":\"Allow\",\"Principal\":{\"AWS\":[\"*\"]},\"Action\":[\"s3:GetObject\"],\"Resource\":[\"arn:aws:s3:::" + bucketName + "/*\"]}]}");
                //}
                //else
                {
                    throw new Exception($"不存在此存储桶：{bucketName}");
                }
            }
        }

        private string[] GetResult(string name)
        {
            if ((name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)) &&
                    !string.IsNullOrWhiteSpace(imageProxyUrl))
                return new string[] { $"http://{this.domainAndPort}/{bucketName}/{name}", $"{imageProxyUrl}//{bucketName}/{name}" };
            else
                return new string[] { $"http://{this.domainAndPort}/{bucketName}/{name}" };
        }

        public async Task<string[]> UploadAsync(string filePath, string name = null, string contentType = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException("filePath");
            if (string.IsNullOrWhiteSpace(name))
            {
                var ext = Path.GetExtension(filePath);
                name = GetRandomName() + ext;
            }
            name = GetSavePath() + name;
            try
            {
                await PrepareBucket();
                await minio.PutObjectAsync(bucketName, name, filePath, ConvertImageContentType(contentType, name));
                return GetResult(name);
            }
            catch (Minio.Exceptions.MinioException e)
            {
                throw new Exception($"上传到 MinIO 时遇到问题：{e.Message}", e);
            }
            catch (Exception e)
            {
                throw new Exception($"上传到 MinIO 时遇到问题：{e.Message}", e);
            }
        }
    }
}
