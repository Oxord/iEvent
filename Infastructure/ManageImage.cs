using iEvent.Domain;
using iEvent.Domain.Models;
using iEvent.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.Json;

namespace iEvent.Infastructure
{
    internal class ManageImage : IManageImage
    {
        private readonly ApplicationDbContext _context;
        public ManageImage(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<string> UploadFiles(List<IFormFile> _IFormFile, Event CurrentEvent)
        {
            string FileName = "";
            try
            {
                List<int> photosID = new();
                foreach (IFormFile file in _IFormFile)
                {
                    FileInfo _FileInfo = new FileInfo(file.FileName);
                    FileName = Guid.NewGuid() + "_" + file.FileName;
                    Photo photo = new Photo() { Name = FileName };
                    _context.Photos.Add(photo);
                    _context.SaveChanges();
                    photosID.Add(photo.Id);
                    var _GetFilePath = Common.GetFilePath(FileName);
                    using (var _FileStream = new FileStream(_GetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(_FileStream);
                    }
                }
                CurrentEvent.Images = JsonSerializer.Serialize(photosID);
                return FileName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<string> UploadCommentsFiles(List<IFormFile> _IFormFile, Comment CurrentComment)
        {
            string FileName = "";
            try
            {
                List<int> photosID = new();
                foreach (IFormFile file in _IFormFile)
                {
                    FileInfo _FileInfo = new FileInfo(file.FileName);
                    FileName = Guid.NewGuid() + "_" + file.FileName;
                    Photo photo = new Photo() { Name = FileName };
                    _context.Photos.Add(photo);
                    _context.SaveChanges();
                    photosID.Add(photo.Id);
                    var _GetFilePath = Common.GetFilePath(FileName);
                    using (var _FileStream = new FileStream(_GetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(_FileStream);
                    }
                }
                CurrentComment.Images = JsonSerializer.Serialize(photosID);
                return FileName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> UploadUserPhoto(IFormFile _IFormFile, User currentUser)
        {
            string FileName = "";
            try
            {
                FileInfo _FileInfo = new FileInfo(_IFormFile.FileName);
                FileName = Guid.NewGuid() + "_" + _IFormFile.FileName;
                Photo photo = new Photo() { Name = FileName };
                _context.Photos.Add(photo);
                _context.SaveChanges();
                currentUser.ProfilePhoto = photo.Id;
                var _GetFilePath = Common.GetFilePath(FileName);
                using (var _FileStream = new FileStream(_GetFilePath, FileMode.Create))
                {
                    await _IFormFile.CopyToAsync(_FileStream);
                }
                return FileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<string> UploadProblemFiles(List<IFormFile> _IFormFile, Problem CurrentProblem)
        {
            string FileName = "";
            try
            {
                List<int> photosID = new();
                foreach (IFormFile file in _IFormFile)
                {
                    FileInfo _FileInfo = new FileInfo(file.FileName);
                    FileName = Guid.NewGuid() + "_" + file.FileName;
                    Photo photo = new Photo() { Name = FileName };
                    _context.Photos.Add(photo);
                    _context.SaveChanges();
                    photosID.Add(photo.Id);
                    var _GetFilePath = Common.GetFilePath(FileName);
                    using (var _FileStream = new FileStream(_GetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(_FileStream);
                    }
                }
                CurrentProblem.Images = JsonSerializer.Serialize(photosID);
                return FileName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> UploadProblemCommentsFiles(List<IFormFile> _IFormFile, ProblemComment CurrentProblemComment)
        {
            string FileName = "";
            try
            {
                List<int> photosID = new();
                foreach (IFormFile file in _IFormFile)
                {
                    FileInfo _FileInfo = new FileInfo(file.FileName);
                    FileName = Guid.NewGuid() + "_" + file.FileName;
                    Photo photo = new Photo() { Name = FileName };
                    _context.Photos.Add(photo);
                    _context.SaveChanges();
                    photosID.Add(photo.Id);
                    var _GetFilePath = Common.GetFilePath(FileName);
                    using (var _FileStream = new FileStream(_GetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(_FileStream);
                    }
                }
                CurrentProblemComment.Images = JsonSerializer.Serialize(photosID);
                return FileName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(byte[], string, string)> DownloadFile(Photo currnetPhoto)
        {
            try
            {
                var _GetFilePath = Common.GetFilePath(currnetPhoto.Name);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(_GetFilePath, out var _ContentType))
                {
                    _ContentType = "application/octet-stream";
                }
                var _ReadAllBytesAsync = await File.ReadAllBytesAsync(_GetFilePath);
                return (_ReadAllBytesAsync, _ContentType, Path.GetFileName(_GetFilePath));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(byte[], string, string)> DownloadDefaultUserIcon()
        {
            try
            {
                var _GetFilePath = Common.GetFilePath("DefaultUserIcon.png");
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(_GetFilePath, out var _ContentType))
                {
                    _ContentType = "application/octet-stream";
                }
                var _ReadAllBytesAsync = await File.ReadAllBytesAsync(_GetFilePath);
                return (_ReadAllBytesAsync, _ContentType, Path.GetFileName(_GetFilePath));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

                      
    }
}
