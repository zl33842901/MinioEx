using Minio;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace xLiAd.MinioEx.Test
{
    /// <summary>
    /// docker run -d --name minio -p 9100:9000 -v /data/miniodata:/data -e "MINIO_ROOT_USER=zhanglei" -e "MINIO_ROOT_PASSWORD=1234.abcd" minio/minio server /data
    /// 
    /// docker run -d --name imageproxy -p 8180:8080 -e "AWS_ACCESS_KEY_ID=zhanglei" -e "AWS_SECRET_KEY=1234.abcd" willnorris/imageproxy -baseURL http://172.16.250.147:9100/ -addr 0.0.0.0:8080 -cache "s3://us-east-1/thumbs/images?endpoint=172.16.250.147:9100&disableSSL=1&s3ForcePathStyle=1"
    /// </summary>
    public class UnitTest1
    {
        private readonly string minioserver = "172.16.250.147:9100", imageproxyurl = "http://172.16.250.147:8180",
            accessKey = "crm_admin", secretKey = "crm_123.abc";

        [Fact]
        public async Task Test1()
        {
            var client = new MinioEx(minioserver, "pointapp", accessKey, secretKey, imageproxyurl, DirectoryPolicy.None);
            var result = await client.UploadAsync("D:\\wordcount.txt", "wordcount.txt");
        }

        [Fact]
        public async Task Test2()
        {
            var client = new MinioEx(minioserver, "pointapp", accessKey, secretKey, imageproxyurl, DirectoryPolicy.None);
            var result = await client.UploadAsync("D:\\top.jpg");
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public async Task Test3()
        {
            FileStream sr = new FileStream("D:\\top.jpg", FileMode.Open);

            var client = new MinioEx(minioserver, "crm", accessKey, secretKey, imageproxyurl, DirectoryPolicy.None);
            var result = await client.UploadAsync(sr, "jpg");
            sr.Close();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public async Task Test4()
        {
            FileStream sr = new FileStream("D:\\top.jpg", FileMode.Open);
            byte[] bytes = new byte[sr.Length];
            sr.Read(bytes, 0, bytes.Length);
            sr.Close();

            var client = new MinioEx(minioserver, "pointapp", accessKey, secretKey, imageproxyurl, DirectoryPolicy.None);
            var result = await client.UploadAsync(bytes, "jpg");
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public async Task Test5()
        {
            var minio = new MinioClient(minioserver, "zhanglei", "1234.abcd");
            var result = await minio.GetPolicyAsync("pointapp");
        }

        [Fact]
        public async Task TestBigFile()
        {
            var client = new MinioEx(minioserver, "pointapp", accessKey, secretKey, imageproxyurl, DirectoryPolicy.None);
            var result = await client.UploadAsync("e:\\1.zip");
        }
    }
}
