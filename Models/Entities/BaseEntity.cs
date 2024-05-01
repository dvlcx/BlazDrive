using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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