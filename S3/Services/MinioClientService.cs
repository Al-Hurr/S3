using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel;
using S3.Models;

namespace S3.Services
{
    public class MinioClientService
    {
        private readonly MinioClientSettings _minioClientSettings;
        private readonly S3ObjectService _s3ObjectService;

        public MinioClientService(MinioClientSettings minioClientSettings, S3ObjectService s3ObjectService)
        {
            _minioClientSettings = minioClientSettings;
            _s3ObjectService = s3ObjectService;
        }

        public async Task<List<S3ObjectDto>> GetFilesListAsync(string bucketName)
        {
            var models = await _s3ObjectService
                .GetAll()
                .Where(x => x.BucketName == bucketName)
                .ToListAsync();

            if(models.Count == 0)
            {
                return null;
            }

            return models.Select(S3ObjectDto.FromEntity).ToList();
        }

        public async Task<Tuple<ObjectStat, MemoryStream>> DownloadFileAsync(string bucketName, string fileName)
        {
            var client = GetMinioClient();

            bool isBucketExists = await client.BucketExistsAsync(
                    new BucketExistsArgs()
                    .WithBucket(bucketName));

            if (!isBucketExists)
            {
                return null;
            }

            using MemoryStream stream = new();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(str => str.CopyTo(stream));

            var file = await client.GetObjectAsync(args);

            return Tuple.Create(file, stream);
        }

        public async Task<string> GetPresignedUrlAsync(string bucketName, string fileName)
        {
            var client = GetMinioClient();

            bool isBucketExists = await client.BucketExistsAsync(
                    new BucketExistsArgs()
                    .WithBucket(bucketName));

            if (!isBucketExists)
            {
                return null;
            }

            var reqParams = new Dictionary<string, string> { { "response-content-type", "application/json" } };
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithExpiry(600)
                .WithHeaders(reqParams);

            return await client.PresignedGetObjectAsync(args);
        }

        public async Task<PutObjectResponse> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
        {
            var client = GetMinioClient();
            bool isBucketExists = await client.BucketExistsAsync(
                    new BucketExistsArgs()
                    .WithBucket(bucketName));

            if (!isBucketExists)
            {
                return null;
            }

            string fileName = string.IsNullOrEmpty(prefix)
                ? file.FileName
                : $"{prefix.TrimEnd('/')}/{file.FileName}";

            var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length);

            var result = await client.PutObjectAsync(args);

            var model = _s3ObjectService.GetByName(result.ObjectName);
            if(model == null)
            {
                _s3ObjectService.Create(
                    new S3Object
                    {
                        Name= result.ObjectName,
                        BucketName= bucketName,
                        FilePath = fileName,
                        ContentType = file.ContentType,
                        LastModified = DateTime.UtcNow
                    });
            }
            else
            {
                model.LastModified = DateTime.UtcNow;
                _s3ObjectService.Update(model);
            }

            return result;
        }

        private MinioClient GetMinioClient()
        {
            return new MinioClient()
                              .WithEndpoint(_minioClientSettings.Endpoint)
                              .WithCredentials(_minioClientSettings.AccessKey, _minioClientSettings.SecretKey)
                              .WithSSL()
                              .Build();
        }
    }
}
