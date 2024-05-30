using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazDrive.Models.Entities
{
    public class Folder : BaseEntity
    {
        public string Name { get; set;}
        [ForeignKey("Folder")]
        public Guid? ParentFolderId { get; set; }  
        public DateTime CreationDate { get; set; }    
        public string FullPath { get; set; }

        public Folder ParentFolder { get; set; }
        public ICollection<User> Users { get; set; }

        public Folder(Guid id, string name, Guid? parentFolderId, DateTime creationDate, string fullPath) : base(id)
        {
            this.Name = name;
            this.ParentFolderId = parentFolderId;
            this.CreationDate = creationDate;
            this.FullPath = fullPath;
        }
    }
}