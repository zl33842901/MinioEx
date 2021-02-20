using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace xLiAd.MinioEx
{
    public interface IMinioEx
    {
        Task<string[]> UploadWithNameAsync(byte[] fileContent, string name, string contentType = null);
        Task<string[]> UploadWithNameAsync(Stream fileContent, string name, string contentType = null);
        Task<string[]> UploadAsync(byte[] fileContent, string fileExt, string contentType = null);
        Task<string[]> UploadAsync(Stream fileContent, string fileExt, string contentType = null);
        Task<string[]> UploadAsync(string filePath, string name = null, string contentType = null);
    }
}
