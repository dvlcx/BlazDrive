namespace BlazDrive.Models.ViewModels
{
    public class BlazDriveViewModel
    {
        public List<FolderViewModel> Folders { get; set; } = [];
        public List<FileViewModel> Files { get; set; } = [];
    }
}