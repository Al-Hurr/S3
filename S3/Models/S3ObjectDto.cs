using System.ComponentModel.DataAnnotations;

namespace S3.Models
{
    public class S3ObjectDto
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public string BucketName { get; set; }

        public string ContentType { get; set; }

        public DateTime? LastModified { get; set; }

        public static S3ObjectDto FromEntity(S3Object entity)
        {
            return new S3ObjectDto
            {
                Name = entity.Name,
                FilePath= entity.FilePath,
                BucketName = entity.BucketName,
                ContentType = entity.ContentType,
                LastModified = entity.LastModified
            };
        }
    }
}
