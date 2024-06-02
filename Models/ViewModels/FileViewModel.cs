using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazDrive.Models.ViewModels
{
    public class FileViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
        public float Size { get; set; }
        public DateTime UploadDate { get; set; }    
        public Guid ParentFolderId {get; set;}  
        public Guid RootFolderId { get; set; }    
        public string? Preview { get; set; }    
        public bool IsSelected { get; set; } = false;    
    }
}