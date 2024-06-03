using System.ComponentModel.DataAnnotations.Schema;

namespace BlazDrive.Models.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[]? Avatar { get; set; }
        [ForeignKey("Folder")]
        public Guid RootFolderId { get; set; }

        public Folder RootFolder { get; set; }
    
        public User(Guid id, string name, string email, string password, Guid rootFolderId) : base(id)
        {
            this.Name = name;
            this.Email = email;
            this.Password = password;
            this.RootFolderId = rootFolderId;
            this.Avatar = null;
        }

    }
}