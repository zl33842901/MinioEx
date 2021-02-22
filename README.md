# MinioEx
对 Minio 的简单扩展，方便使用。

### 适用场景

1，需要使用图片缩略图。

2，多系统需要共享图片或其他类型文件。

3，项目需要快速集成图片或文件上传功能。

4，需要对图片或文件进行多租户管理。

### 安装程序包

使用 AspNetCore:

dotnet add package xLiAd.MinioEx.AspNetCore

使用控制台或其他 DotNet 项目:

dotnet add package xLiAd.MinioEx

### 使用方法

1，搭建好 minio 和 imageproxy，imageproxy 不是必须的。本文以下部分的 minio 地址使用 172.16.250.147:9100 作为示例。

```shell
docker run -d --name minio -p 9100:9000 -v /data/miniodata:/data -e "MINIO_ROOT_USER=admin" -e "MINIO_ROOT_PASSWORD=admin_123.abc" minio/minio server /data

docker run -d --name imageproxy -p 8180:8080 -e "AWS_ACCESS_KEY_ID=admin" -e "AWS_SECRET_KEY=admin_123.abc" willnorris/imageproxy -baseURL http://172.16.250.147:9100/ -addr 0.0.0.0:8080 -cache "s3://us-east-1/thumbs/images?endpoint=172.16.250.147:9100&disableSSL=1&s3ForcePathStyle=1"
```

注意把示例中的 IP地址替换为自己服务器的IP，管理员的名称、密码也要替换一下。

2，下载 minio 管理工具 mc.exe，并运行，使用以下命令连接至 minio。

mc config host add minio147 http://172.16.250.147:9100 YourAccessKey YourSecretKey

其中 minio147 是自己命名的别名。

3，编辑 newbucket.bat 文件的第一行，把 minio147 改为上一步命名的别名，并复制到 mc.exe 所在目录。

4，执行 newbucket YourBucketName 新建存储桶（YourBucketName 为你的桶名）。此脚本做了如下几件事（此脚本可多次执行，但桶名不能一样）：

​    a，创建存储桶

​    b，设置存储桶的匿名权限为只读

​    c，新增管理员 YourBucketName_admin ，密码为 YourBucketName_123.abc

​    d，新增管理策略 YourBucketName_admin_policy，此策略对创建的存储桶具有读写权限

​    e，将管理策略 YourBucketName_admin_policy 应用到管理员 YourBucketName_admin。

5，如果您的项目是 AspNetCore 项目，需要在 Startup 类的 ConfigureServices 方法里加入：

```csharp
            services.AddMinioEx((sp, options) =>
            {
                var conf = sp.GetService<IConfiguration>();
                options.MinioUrl = conf.GetSection("MinioUrl").Value;
                options.BucketName = conf.GetSection("BucketName").Value;
                options.AccessKey = conf.GetSection("AccessKey").Value;
                options.SecretKey = conf.GetSection("SecretKey").Value;
                options.ImageProxyUrl = conf.GetSection("ImageProxyUrl").Value;
                options.DirectoryPolicy = DirectoryPolicy.ByMonth;
            });
```

并在配置文件中加入：

```json
{
  "MinioUrl": "172.16.250.147:9100",
  "BucketName": "YourBucketName",
  "AccessKey": "YourBucketName_admin",
  "SecretKey": "YourBucketName_123.abc",
  "ImageProxyUrl": "http://172.16.250.147:8180" /*ImageProxy 地址*/
}
```

然后在依赖注入中得到 IMinioEx 实例。

如果您的项目不是 AspNetCore 而是一般的 dotnet 程序，您需要自己实例化对象：

```csharp
var client = new MinioEx("172.16.250.147:9100", "YourBucketName", "YourBucketName_admin", "YourBucketName_123.abc", "http://172.16.250.147:8180", DirectoryPolicy.None);
```

6，得到实例后，使用 UploadAsync 或 UploadWithNameAsync 方法执行上传操作。

7，上传方法会返回上传后的路径，如果是图片，还会返回 ImageProxy 代理的路径。返回类型是字符串数组，第一个元素固定是 minio 中的地址，如果存在第二个元素，则是 ImageProxy 的文件路径（需要是图片），在路径中增加 option 信息可以对图片进行缩略。

