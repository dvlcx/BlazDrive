using System.ComponentModel.DataAnnotations;

namespace BlazDrive.Models.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set;}
        
        public BaseEntity(Guid id)
        {
            this.Id = id;
        }
    }
}