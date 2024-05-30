using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using BlazDrive.Models;
using BlazDrive.Models.Entities;
using BlazDrive.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Services
{
    public class BlazDriveStorageService
    {
        private FolderRepository _folderRepo;
        private FileRepository _fileRepo;

        public BlazDriveViewModel BlazDriveViewModel { get; set; } = new BlazDriveViewModel();

        public BlazDriveStorageService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _folderRepo = new FolderRepository(contextFactory);
            _fileRepo = new FileRepository(contextFactory);
        }

        public async Task RefreshAsync(Guid folderId)
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
                        CreationDate = file.CreationDate,
                        UploadDate = file.UploadDate,
                        ParentFolderId = file.ParentFolderId,
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
            Guid? tmp = Guid.Empty;
            var id = parentFolder;
            while (tmp is not null)
            {
                tmp = (await _folderRepo.GetByIdAsync((Guid)tmp))?.ParentFolderId;
                if (tmp is not null)
                {
                    path = $"{tmp}/" + path;
                }         
            }

            Directory.CreateDirectory("Storage/" + path + nid);

            await _folderRepo.AddAsync(new Folder(
                nid,
                name,
                parentFolder,
                DateTime.Now,
                path
            ));
        }
    }
}