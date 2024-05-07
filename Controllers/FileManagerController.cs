
using iEvent.Domain;
using iEvent.Infastructure;
using Microsoft.AspNetCore.Mvc;

namespace iEvent.Controllers
{
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly IManageImage _iManageImage;
        private readonly ApplicationDbContext _context;

        public FileManagerController(IManageImage iManageImage, ApplicationDbContext context)
        {
            _iManageImage = iManageImage;
            _context = context;
        }


        [HttpGet]
        [Route("Downloadhoto")]
        public async Task<IActionResult> DownloadPhoto(int photoId)
        {
            var currnetPhoto = _context.Photos.FirstOrDefault(x => x.Id == photoId);
            if (currnetPhoto != null)
            {
                var result = await _iManageImage.DownloadFile(currnetPhoto);
                return File(result.Item1, result.Item2, result.Item2);
            }
            return NotFound();
        }

    }
}
