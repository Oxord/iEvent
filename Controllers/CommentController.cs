using iEvent.Domain;
using iEvent.Domain.Models;
using iEvent.Domain.Repositories;
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
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProblemCommentRepository _problemCommentRepository;
        public CommentController(UserManager<User> userManager, 
            IConfiguration configuration, ApplicationDbContext context, 
            IManageImage iManageImage, ICommentRepository commentRepository,
            IUnitOfWork unitOfWork, IProblemCommentRepository problemCommentRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _iManageImage = iManageImage;
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _problemCommentRepository = problemCommentRepository;
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
                    AuthorId = user.Id,
                    authorImage = user.ProfilePhoto,
                    Images = "",
                    authorLogin = user.Id,
                };
                _commentRepository.AddComment(com);
                _unitOfWork.Commit();
                return Ok();
            }

            return BadRequest(StatusCodes.Status500InternalServerError);
        }

        [Authorize]
        [HttpPut("AddImagesToComment")]
        public async Task<ActionResult> AddImagesToCommentToEvent(List<IFormFile> _IFormFile, int comentId)
        {
            var current_comment = _context.Comments.FirstOrDefault(x => x.Id == comentId);
            if (current_comment != null)
            {
                var result = await _iManageImage.UploadCommentsFiles(_IFormFile, current_comment);
                _unitOfWork.Commit();
                return Ok(result);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPut("EditComment")]
        public async Task<ActionResult> EditComment(int comentId, [FromBody] CreateCommentModel model)
        {
            var current_comment = _context.Comments.FirstOrDefault(x => x.Id == comentId);
            var user = await GetCurrentUserAsync();
            if (current_comment != null && user != null)
            {
                if(current_comment.authorLogin == user.Id)
                {
                    current_comment.Text = model.Text;
                    _unitOfWork.Commit();
                    return Ok();
                }
                return Forbid();
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete("DeleteComment")]
        public async Task<ActionResult> DeleteComment(int comentId)
        {
            var current_comment = _context.Comments.FirstOrDefault(x => x.Id == comentId);
            var user = await GetCurrentUserAsync();
            if (current_comment != null && user != null)
            {
                if (current_comment.authorLogin == user.Id)
                {
                    _context.Comments.Remove(current_comment);
                    _unitOfWork.Commit();
                    return Ok();
                }
                return Forbid();
            }
            return NotFound();
        }

        [Authorize(Policy = "PeopleCanSolve")]
        [HttpPost("CreateCommentByProblem")]
        public async Task<ActionResult<Comment>> CreateCommentByProblem([FromBody] CreateCommentModel model, int problemId)
        {
            var user = await GetCurrentUserAsync();
            var CurrentProblem = GetProblemById(problemId);
            if (CurrentProblem != null && user != null)
            {
                ProblemComment com = new()
                {
                    Text = model.Text,
                    Problem = CurrentProblem,
                    AuthorId = user.Id,
                    authorImage = user.ProfilePhoto,
                    Images = "",
                    authorLogin = user.Id,
                };
                _problemCommentRepository.AddComment(com);
                _unitOfWork.Commit();
                return Ok();
            }

            return Forbid();
        }

        [Authorize(Policy = "PeopleCanSolve")]
        [HttpPut("AddImagesToCommentToProblem")]
        public async Task<ActionResult> AddImagesToCommentToProblem(List<IFormFile> _IFormFile, int comentId)
        {
            var current_comment = _context.ProblemComments.FirstOrDefault(x => x.Id == comentId);
            if (current_comment != null)
            {
                var result = await _iManageImage.UploadProblemCommentsFiles(_IFormFile, current_comment);
                _unitOfWork.Commit();
                return Ok(result);
            }
            return Forbid();
        }

        [Authorize(Policy = "PeopleCanSolve")]
        [HttpPut("EditProblemComment")]
        public async Task<ActionResult> EditProblemComment(int comentId, [FromBody] CreateCommentModel model)
        {
            var current_comment = _context.ProblemComments.FirstOrDefault(x => x.Id == comentId);
            var user = await GetCurrentUserAsync();
            if (current_comment != null && user != null)
            {
                if (current_comment.authorLogin == user.Id)
                {
                    current_comment.Text = model.Text;
                    _unitOfWork.Commit();
                    return Ok();
                }
                return Forbid();
            }
            return NotFound(); ;
        }

        [Authorize(Policy = "PeopleCanSolve")]
        [HttpDelete("DeleteProblemComment")]
        public async Task<ActionResult> DeleteProblemComment(int comentId)
        {
            var current_comment = _context.ProblemComments.FirstOrDefault(x => x.Id == comentId);
            var user = await GetCurrentUserAsync();
            if (current_comment != null && user != null)
            {
                if (current_comment.authorLogin == user.Id)
                {
                    _context.ProblemComments.Remove(current_comment);
                    _unitOfWork.Commit();
                    return Ok();
                }
                return Forbid();
            }
            return NotFound();
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
