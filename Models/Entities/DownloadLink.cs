using System.ComponentModel.DataAnnotations.Schema;

namespace BlazDrive.Models.Entities
{
    public class DownloadLink : BaseEntity
    {
        public Guid FileId { get; set; }
        public string FileName {get; set;}
        public string? Password { get; set; }
        public DateTime? ExpireTime { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public User User { get; set; }

        public DownloadLink(Guid id, string fileName, Guid userId, DateTime? expireTime, string? password, Guid fileId) : base(id)
        {
            this.UserId = userId;
            this.ExpireTime = expireTime;
            this.Password = password;
            this.FileId = fileId;
            this.FileName = fileName;
        }
    }
}