using System.ComponentModel.DataAnnotations.Schema;

namespace BlazDrive.Models.Entities
{
    public class File : BaseEntity
    {
        public string Name { get; set; }
        public FileType Type { get; set; }
        public float Size { get; set; }
         public DateTime UploadDate { get; set; }    
        [ForeignKey("Folder")]
        public Guid ParentFolderId {get; set;}

        public Folder Folder { get; set; }

        public File(Guid id, string name, FileType type, float size, DateTime UploadDate, Guid parentFolderId) : base(id)
        {
            this.Name = name;
            this.Type = type;
            this.Size = size;
            this.UploadDate = UploadDate;
            this.ParentFolderId = parentFolderId;
        }
    }
}