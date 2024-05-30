using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Models.Entities;

namespace BlazDrive.Models.ViewModels
{
    public class BlazDriveViewModel
    {
        public List<FolderViewModel> Folders { get; set; } = [];
        public List<FileViewModel> Files { get; set; } = [];
    }
}