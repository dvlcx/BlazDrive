using System.ComponentModel.DataAnnotations.Schema;

namespace BlazDrive.Models.Entities
{
    public class File : BaseEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public float Size { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UploadDate { get; set; }    
        [ForeignKey("Folder")]
        public Guid ParentFolderId {get; set;}

        public Folder Folder { get; set; }

        public File(Guid id, string name, string type, float size, DateTime creationDate, DateTime UploadDate) : base(id)
        {
            this.Name = name;
            this.Type = type;
            this.Size = size;
            this.CreationDate = creationDate;
            this.UploadDate = UploadDate;
        }
    }
}