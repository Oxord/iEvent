using iEvent.Domain;
using iEvent.Domain.Models;
using iEvent.DTO;
using iEvent.DTO.CommentDto;
using iEvent.Infastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Controllers
{

    public class CommentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IManageImage _iManageImage;
        public CommentController(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext context, IManageImage iManageImage)

        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _iManageImage = iManageImage;
        }
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize]
        [HttpPost("CreateCommentByEvent")]
        public async Task<ActionResult<Comment>> CreateCommentByEvent([FromBody] CreateCommentModel model, int eventId)
        {
            var user = await GetCurrentUserAsync();
            var CurrentEvent = GetEventById(eventId);
            if (CurrentEvent != null)
            {
                Comment com = new()
                {
                    Text = model.Text,
                    Event = CurrentEvent,
                    authorName = user.Name,
                    authorSurname = user.Surname,
                    authorImage = user.ProfilePhoto,
                    Images = "",
                };
                _context.Comments.Add(com);
                _context.SaveChanges();
                return Ok();
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Unlucky" });
        }

        [Authorize]
        [HttpPut("AddImagesToComment")]
        public async Task<ActionResult> AddImagesToCommentToEvent(List<IFormFile> _IFormFile, int comentId)
        {
            var current_comment = _context.Comments.FirstOrDefault(x => x.Id == comentId);
            if (current_comment != null)
            {
                var result = await _iManageImage.UploadCommentsFiles(_IFormFile, current_comment);
                _context.SaveChanges();
                return Ok(result);
            }
            return BadRequest("Такого комментария нету");
        }

        [Authorize]
        [HttpPost("CreateCommentByProblem")]
        public async Task<ActionResult<Comment>> CreateCommentByProblem([FromBody] CreateCommentModel model, int problemId)
        {
            var user = await GetCurrentUserAsync();
            var CurrentProblem = GetProblemById(problemId);
            if (CurrentProblem != null)
            {
                ProblemComment com = new()
                {
                    Text = model.Text,
                    Problem = CurrentProblem,
                    authorName = user.Name,
                    authorSurname = user.Surname,
                    authorImage = user.ProfilePhoto,
                    Images = "",
                };
                _context.ProblemComments.Add(com);
                _context.SaveChanges();
                return Ok();
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Unlucky" });
        }

        [Authorize]
        [HttpPut("AddImagesToCommentToProblem")]
        public async Task<ActionResult> AddImagesToCommentToProblem(List<IFormFile> _IFormFile, int comentId)
        {
            var current_comment = _context.ProblemComments.FirstOrDefault(x => x.Id == comentId);
            if (current_comment != null)
            {
                var result = await _iManageImage.UploadProblemCommentsFiles(_IFormFile, current_comment);
                _context.SaveChanges();
                return Ok(result);
            }
            return BadRequest("Такого комментария нету");
        }


        private Event GetEventById(int eventId)
        {
            Event CurrentEvent = null;
            foreach (Event ev in _context.TableEvents)
            {
                if (ev.Id == eventId)
                {
                    CurrentEvent = ev;
                    break;
                }
            }
            return CurrentEvent;
        }
        private Problem GetProblemById(int problemId)
        {
            Problem CurrentProblem = null;
            foreach (Problem pr in _context.Problems)
            {
                if (pr.Id == problemId)
                {
                    CurrentProblem = pr;
                    break;
                }
            }
            return CurrentProblem;
        }

    }
}
