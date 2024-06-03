namespace BlazDrive.Models.ViewModels
{
    public class FolderViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set;}
        public Guid? ParentFolderId { get; set; }  
        public DateTime CreationDate { get; set; }
        public string ViewPath{ get; set; }
        public bool IsSelected { get; set; } = false;    
    }
}