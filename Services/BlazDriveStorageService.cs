using System.IO.Compression;
using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using BlazDrive.Models;
using BlazDrive.Models.Entities;
using BlazDrive.Models.OutputModels;
using BlazDrive.Models.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazDrive.Services
{
    public class BlazDriveStorageService
    {
        private FolderRepository _folderRepo;
        private FileRepository _fileRepo;
        private FileEncryptionService _fileEncryption;
        private IMemoryCache _cache;

        public BlazDriveViewModel BlazDriveViewModel { get; set; } = new BlazDriveViewModel();
        public List<Folder> FoldersToRemove { get; set; } = [];


        public BlazDriveStorageService(IDbContextFactory<AppDbContext> contextFactory, FileEncryptionService fileEncryptionService, IMemoryCache cache)
        {
            _folderRepo = new FolderRepository(contextFactory);
            _fileRepo = new FileRepository(contextFactory);
            _fileEncryption = fileEncryptionService;
            _cache = cache;
        }

        public async Task RefreshFullAsync(Guid folderId)
        {
            BlazDriveViewModel = new();
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
                    RootFolderId = Guid.Parse(await this.GetFolderRootFolder(folder.Id)),
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

        public async Task MoveFolder(Guid folderId, Guid destinationFolderId)
        {
            var folder  = await _folderRepo.GetByIdAsync(folderId);
            folder.ParentFolderId = destinationFolderId;
            await _folderRepo.UpdateAsync(folder);
        }

        public async Task CopyFolder(Guid folderId, Guid destinationFolderId)
        {
            var folder  = await _folderRepo.GetByIdAsync(folderId);
            folder.ParentFolderId = destinationFolderId;
            folder.Id = Guid.NewGuid();
            folder.CreationDate = DateTime.Now;
            await _folderRepo.AddAsync(folder);
            await CopyFolderRecursive(folder.Id);
        }

        private async Task CopyFolderRecursive(Guid folderId)
        {
            var folders = await _folderRepo.GetByParentId(folderId);
            var files = await _fileRepo.GetByFolderId(folderId);

            foreach (var file in files)
            {
                file.ParentFolderId = folderId;
                file.Id = Guid.NewGuid();
                await _fileRepo.AddAsync(file);
            }

            foreach (var folder in folders)
            {
                folder.Id = Guid.NewGuid();
                folder.ParentFolderId = folderId;
                folder.CreationDate = DateTime.Now;
                await _folderRepo.AddAsync(folder);
                await CopyFolderRecursive(folder.Id);
            }
        }

        public async Task MoveFile(Guid fileId, Guid destinationFolderId)
        {
            var file  = await _fileRepo.GetByIdAsync(fileId);
            file.ParentFolderId = destinationFolderId;
            await _fileRepo.UpdateAsync(file);
        }

        public async Task CopyFile(Guid fileId, Guid destinationFolderId)
        {
            var file  = await _fileRepo.GetByIdAsync(fileId);
            file.ParentFolderId = destinationFolderId;
            file.Id = Guid.NewGuid();
            await _fileRepo.AddAsync(file);
        }

        public async Task RenameFile(Guid fileId, string newName)
        {
            var f = await _fileRepo.GetByIdAsync(fileId);
            f.Name = newName;
            await _fileRepo.UpdateAsync(f);
        }
        
        public async Task RenameFolder(Guid folderId, string newName)
        {
            var f = await _folderRepo.GetByIdAsync(folderId);
            f.Name = newName;
            await _folderRepo.UpdateAsync(f);
        }

        public async Task<Guid> PrepareDownloadFile(Guid fileId, Guid rootFolderId, string fileName, Guid userId)
        {
            byte[] fileArr = await _fileEncryption.DecryptFile(System.IO.File.ReadAllBytes($"Storage/{rootFolderId}/{fileId}"));

            var fileModel = new OutputFile()
            {
                FileName = fileName,
                File = fileArr,
                UserId = userId,
            };

            var key = Guid.NewGuid();
            _cache.Set(key, fileModel);

            return key;
        }

        public async Task<Guid> PrepareDownloadFolder(Guid rootFolderId, Guid folderId, string folderName,  Guid userId)
        {
            using (var compressedFileStream = new MemoryStream())   
            {
                using (ZipArchive zip = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    foreach (var file in await _fileRepo.GetByFolderId(folderId))
                    {
                        var zipEntry = zip.CreateEntry(file.Name);
                        byte[] fileArr = await _fileEncryption.DecryptFile(System.IO.File.ReadAllBytes($"Storage/{rootFolderId}/{file.Id}"));
                        using (var originalFileStream = new MemoryStream(fileArr))
                        using (var zipEntryStream = zipEntry.Open()) {
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                    }

                    foreach (var folder in await _folderRepo.GetByParentId(folderId))
                    {
                        await PrepareDownloadFolderRecursive(rootFolderId, zip, folder.Id);
                    }
                }
                var fileModel = new OutputFile()
                {
                    FileName = folderName + ".zip",
                    File = compressedFileStream.ToArray(),
                    UserId = userId,
                };
                    var key = Guid.NewGuid();
                _cache.Set(key, fileModel);

                return key;
            }
        }

        private async Task PrepareDownloadFolderRecursive(Guid rootFolderId, ZipArchive zip, Guid folderId)
        {
            var folders = await _folderRepo.GetByParentId(folderId);
            var files = await _fileRepo.GetByFolderId(folderId);

            foreach (var file in files)
            {
                var zipEntry = zip.CreateEntry((await this.GetFilePath(file)).Skip(1).Aggregate((i, j) => i + j) + file.Name);
                byte[] fileArr = await _fileEncryption.DecryptFile(System.IO.File.ReadAllBytes($"Storage/{rootFolderId}/{file.Id}"));
                using (var originalFileStream = new MemoryStream(fileArr))
                    using (var zipEntryStream = zipEntry.Open())
                        originalFileStream.CopyTo(zipEntryStream);
            }

            foreach (var folder in folders)
            {
                await PrepareDownloadFolderRecursive(rootFolderId, zip, folder.Id);
            }
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

        private async Task<List<string>> GetFilePath(Models.Entities.File file)
        {
            List<string> path = [];
            Guid? tmp = file.ParentFolderId;
            while (tmp is not null)
            {
                var folder = await _folderRepo.GetByIdAsync((Guid)tmp);
                tmp = folder?.ParentFolderId;
                if (tmp is not null)
                {
                    path.Add($"{folder.Name}/");
                }         
            }
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

        private async Task<string> GetFolderRootFolder(Guid folderId)
        {
            string rootFolderId = folderId.ToString();
            Guid? tmp = (await _folderRepo.GetByIdAsync(folderId)).ParentFolderId;
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