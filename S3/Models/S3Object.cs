using System.ComponentModel.DataAnnotations;

namespace S3.Models
{
    public class S3Object
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }

        public string BucketName { get; set; }

        public string ContentType { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
