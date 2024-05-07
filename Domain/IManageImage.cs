using iEvent.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.Domain
{
    public interface IManageImage
    {
        Task<string> UploadUserPhoto(IFormFile _IFormFile, User CurrentUser);
        Task<string> UploadFiles(List<IFormFile> _IFormFile, Event CurrentEvent);
        Task<string> UploadCommentsFiles(List<IFormFile> _IFormFile, Comment CurrentComment);
        Task<string> UploadProblemFiles(List<IFormFile> _IFormFile, Problem CurrentProblem);
        Task<string> UploadProblemCommentsFiles(List<IFormFile> _IFormFile, ProblemComment CurrentProblemComment);
        Task<(byte[], string, string)> DownloadFile(Photo currnetPhoto);
        Task<(byte[], string, string)> DownloadDefaultUserIcon();

    }
}
