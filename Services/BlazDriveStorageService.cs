using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using BlazDrive.Models;
using BlazDrive.Models.Entities;
using BlazDrive.Models.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Services
{
    public class BlazDriveStorageService
    {
        private FolderRepository _folderRepo;
        private FileRepository _fileRepo;
        private FileEncryptionService _fileEncryption;

        public BlazDriveViewModel BlazDriveViewModel { get; set; } = new BlazDriveViewModel();
        public List<Folder> FoldersToRemove { get; set; } = [];


        public BlazDriveStorageService(IDbContextFactory<AppDbContext> contextFactory, FileEncryptionService fileEncryptionService)
        {
            _folderRepo = new FolderRepository(contextFactory);
            _fileRepo = new FileRepository(contextFactory);
            _fileEncryption = fileEncryptionService;
        }

        public async Task RefreshFullAsync(Guid folderId)
        {
            var folder = await _folderRepo.GetByIdAsync(folderId);
            var files = await _fileRepo.GetByFolderId(folderId);

            BlazDriveViewModel.Folders.Add( 
                new FolderViewModel()
            {
                Id = folder.Id,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId,
                CreationDate = folder.CreationDate,
            });

            await this.RefreshAsync(folder.Id);

            foreach (var file in BlazDriveViewModel.Files.Where(f => f.Type is FileType.Image))
            {
                byte[] imageArray = await _fileEncryption.DecryptFile(System.IO.File.ReadAllBytes($"Storage/{file.RootFolderId}/{file.Id}"));
                file.Preview = Convert.ToBase64String(imageArray);
            }
        }

        private async Task RefreshAsync(Guid folderId)
        {
            var folders = await _folderRepo.GetByParentId(folderId);
            var files = await _fileRepo.GetByFolderId(folderId);

            foreach (var file in files)
            {
                BlazDriveViewModel.Files.Add(
                    new FileViewModel()
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Type = file.Type,
                        Size = file.Size,
                        UploadDate = file.UploadDate,
                        ParentFolderId = file.ParentFolderId,
                        RootFolderId = Guid.Parse(await this.GetFileRootFolder(file.Id)),
                    }
                );
            }

            foreach (var folder in folders)
            {
                BlazDriveViewModel.Folders.Add( new FolderViewModel()
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    ParentFolderId = folder.ParentFolderId,
                    CreationDate = folder.CreationDate,
                });
                await RefreshAsync(folder.Id);
            }
        }

        public async Task CreateFolder(Guid parentFolder, string name)
        {
            var nid = Guid.NewGuid();
            string path = $"{parentFolder}/";
            Guid? tmp = parentFolder;
            while (tmp is not null)
            {
                tmp = (await _folderRepo.GetByIdAsync((Guid)tmp))?.ParentFolderId;
                if (tmp is not null)
                {
                    path = $"{tmp}/" + path;
                }         
            }

            // Directory.CreateDirectory("Storage/" + path + nid);

            await _folderRepo.AddAsync(new Folder(
                nid,
                name,
                parentFolder,
                DateTime.Now,
                path
            ));
        }

        public async Task DeleteFolder(Guid folderId)
        {
            var folder = await _folderRepo.GetByIdAsync(folderId);
            var files = await _fileRepo.GetByFolderId(folderId);

            foreach (var file in files)
            {
                System.IO.File.Delete($"Storage/{await GetFileRootFolder(file.Id)}/{file.Id}");
                _fileRepo.Delete(file);
            }
            
            FoldersToRemove.Add(folder);

            await this.DeleteFolderRecursive(folder.Id);

            FoldersToRemove.Reverse();
            
            // Directory.Delete((await this.GetFolderPath(folderId)).Aggregate((i, j) => i + j), true);

            foreach (var x in FoldersToRemove)
            {                
                _folderRepo.Delete(x);
            }
            FoldersToRemove = [];
        }

        public async Task DeleteFolderRecursive(Guid folderId)
        {
            var folders = await _folderRepo.GetByParentId(folderId);
            var files = await _fileRepo.GetByFolderId(folderId);

            foreach (var file in files)
            {
                _fileRepo.Delete(file);
            }

            foreach (var folder in folders)
            {
                FoldersToRemove.Add(folder);
                await DeleteFolderRecursive(folder.Id);
            }
        }

        public async Task UploadFile(IReadOnlyList<IBrowserFile> files, string rootId, Guid parentFolderId)
        {
            foreach (var file in files)
            {
                using (var ms = new MemoryStream())
                {
                    await file.OpenReadStream().CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    var encrypted = await _fileEncryption.EncryptFile(fileBytes);
                    
                
                    var id = Guid.NewGuid();
                    using (var bw = new BinaryWriter(System.IO.File.Open($"Storage/{rootId}/{id}", FileMode.Create)))
                    {
                        bw.Write(encrypted);
                        // await file.OpenReadStream().CopyToAsync(fs);
                        FileType type = 
                        file.Name.Split('.').Last() switch 
                        {
                            "bmp" or "png" or "jpg" or "jpeg" or "webp" or "svg" =>  FileType.Image,
                            "mp4" or "avi" or "wmv" or "mkv" or "mov" or "amv" or "mpg" =>  FileType.Video,
                            "wav" or "mp3" or "wma" or "au" or "flac" or "wv" or "m4b" or "m4a" =>  FileType.Audio,
                            "doc" or "docx" or "odt" or "pdf" or "xls" or "xlsx" or "ods" or "ppt" or "pptx" or "txt" =>  FileType.Document,
                            "exe" or "bat" or "cmd" or "msi" or "ps1" or "apk" or "bin" or "sh" => FileType.Executable,
                            "zip" or "iso" or "rar" or "lz" or "gz" or "br" or "bz2" or "7z" => FileType.Archive,
                            _ => FileType.Other,
                        };
                        await _fileRepo.AddAsync(new Models.Entities.File(
                        id,
                        file.Name,
                        type,
                        file.Size,
                        DateTime.Now,
                        parentFolderId
                        ));
                    }
                }
            }
        }

        public async Task DeleteFile(Guid fileId)
        {
            // System.IO.File.Delete((await GetFilePath(fileId)).Aggregate((i, j) => i + j));
            System.IO.File.Delete($"Storage/{await GetFileRootFolder(fileId)}/{fileId}");
            await _fileRepo.DeleteByIdAsync(fileId);
        }

        private async Task<List<string>> GetFolderPath(Guid folderId)
        {
            List<string> path = [$"{folderId}"];
            Guid? tmp = folderId;
            while (tmp is not null)
            {
                tmp = (await _folderRepo.GetByIdAsync((Guid)tmp))?.ParentFolderId;
                if (tmp is not null)
                {
                    path.Add($"{tmp}/");
                }         
            }
            path.Add("Storage/");
            path.Reverse();
            return path;
        }

        private async Task<List<string>> GetFilePath(Guid fileId)
        {
            List<string> path = [$"{fileId}"];
            Guid? tmp = (await _fileRepo.GetByIdAsync(fileId)).ParentFolderId;
            while (tmp is not null)
            {
                tmp = (await _folderRepo.GetByIdAsync((Guid)tmp))?.ParentFolderId;
                if (tmp is not null)
                {
                    path.Add($"{tmp}/");
                }         
            }
            path.Add("Storage/");
            path.Reverse();
            return path;
        }

        private async Task<string> GetFileRootFolder(Guid fileId)
        {
            string rootFolderId = string.Empty;
            Guid? tmp = (await _fileRepo.GetByIdAsync(fileId)).ParentFolderId;
            while (tmp is not null)
            {
                var res = (await _folderRepo.GetByIdAsync((Guid)tmp))?.ParentFolderId;
                if (res is null)
                {
                    rootFolderId = tmp.ToString();
                }
                tmp = res;  
            }
            return rootFolderId;
        }
    }
}