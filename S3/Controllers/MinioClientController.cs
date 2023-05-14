using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using S3.Models;
using S3.Services;
using System.Reactive.Linq;

namespace S3.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MinioClientController : ControllerBase
    {
        private readonly MinioClientService _minioClientService;

        public MinioClientController(MinioClientService minioClientService)
        {
            _minioClientService = minioClientService;
        }

        [HttpGet]
        [Route("GetFilesList")]
        public async Task<IActionResult> GetFilesListAsync(string bucketName)
        {
            var result = await _minioClientService.GetFilesListAsync(bucketName);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFileAsync(string bucketName, string fileName)
        {
            var result = await _minioClientService.DownloadFileAsync(bucketName, fileName);

            if (result == null)
            {
                return NotFound($"Bucket {bucketName} does not found");
            }

            var objectStat = result.Item1;
            var stream = result.Item2;

            new FileExtensionContentTypeProvider()
                .TryGetContentType(objectStat.ObjectName, out string? contentType);

            return File(stream.ToArray(), contentType ?? objectStat.ContentType, objectStat.ObjectName);
        }

        [HttpGet]
        [Route("GetPresignedUrl")]
        [ProducesResponseType(typeof(S3ObjectDto), 200)]
        public async Task<IActionResult> GetPresignedUrlAsync(string bucketName, string fileName)
        {
            var result = await _minioClientService.GetPresignedUrlAsync(bucketName, fileName);

            if (result == null)
            {
                return NotFound($"Bucket {bucketName} does not found");
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
        {
            var result = await _minioClientService.UploadFileAsync(file, bucketName, prefix);

            if (result == null)
            {
                return NotFound($"Bucket {bucketName} does not found");
            }

            return Ok($"File {result.ObjectName} uploaded successfully");
        }
    }
}