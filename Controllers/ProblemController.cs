using iEvent.DTO;
using iEvent.DTO.CommentDto;
using iEvent.DTO.ProblemDto;
using iEvent.Domain.Models;
using iEvent.Infastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using iEvent.Auth.ProblemDto;
using iEvent.Domain;
using System.Text.Json;
using System.Xml.Linq;
using iEvent.Domain.Repositories;

namespace iEvent.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProblemController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IManageImage _iManageImage;
        private readonly IProblemRepository _problemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProblemController(UserManager<User> userManager, IConfiguration configuration,
            ApplicationDbContext context, IManageImage iManageImage,
            IUnitOfWork unitOfWork, IProblemRepository problemRepository)

        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _iManageImage = iManageImage;
            _unitOfWork = unitOfWork;
            _problemRepository = problemRepository;
        }



        [Authorize(Policy = "ProblemCreators")]
        [HttpPost(Name = "CreateProblem")]
        public async Task<ActionResult> CreateProblem(CreateProblemModel model)
        {
            var user = await GetCurrentUserAsync();
            Problem problem = new()
            {
                Title = model.Title,
                DescriptionText = model.DescriptionText,
                Category = model.Category,
                AuthorId = user.Id,
                authorImage = user.ProfilePhoto,
                Images = "",
            };
            _problemRepository.AddProblem(problem);
            _unitOfWork.Commit();
            return Ok();

        }

        [Authorize(Policy = "ProblemCreators")]
        [HttpPut(Name = "EditProblem")]
        public async Task<ActionResult> EditProblem([FromBody] CreateProblemModel model, int problemId)
        {
            var user = await GetCurrentUserAsync();
            var current_problem = _context.Problems.FirstOrDefault(x => x.Id == problemId);
            if (current_problem != null && user != null)
            {
                if (current_problem.AuthorId == user.Id)
                {
                    current_problem.Title = model.Title;
                    current_problem.DescriptionText = model.DescriptionText;
                    current_problem.Category = model.Category;
                    _unitOfWork.Commit();
                    return Ok();
                }
                return Forbid();
            }
            return NotFound();
        }

        [HttpGet(Name = "GetAllProblems")]
        public async Task<ActionResult<List<ProblemInList>>> GetAllProblems()
        {
            return _problemRepository.GetProblems();
        }

        [HttpGet(Name = "GetProblem")]
        public async Task<ActionResult<ProblemOnly>> GetProblem(int problemId)
        {
            var current_problem = _context.Problems.FirstOrDefault(x => x.Id == problemId);
            var Comments = new List<ProblemComment>(); 
            
            if (current_problem != null)
            {
                foreach (ProblemComment com in _context.ProblemComments.ToList().Where(x => x.Problem == current_problem))
                {
                    Comments.Add(com);
                }
                List<ViewCommentModel> comments = new();
                foreach (var com in Comments)
                {
                    User author = _context.Users.FirstOrDefault(x => x.Id == com.AuthorId);
                    List<int>? com_photosId = new();
                    if (com.Images != "") 
                    { 
                        com_photosId = JsonSerializer.Deserialize<List<int>>(com.Images);
                        comments.Add(new ViewCommentModel
                        {
                            authorName = author.Name,
                            authorSurname = author.Surname,
                            authorAvatar = com.authorImage,
                            Text = com.Text,
                            photos = com_photosId,
                        });
                    }
                    else
                    {
                        
                        comments.Add(new ViewCommentModel
                        {
                            authorName = author.Name,
                            authorSurname = author.Surname,
                            authorAvatar = com.authorImage,
                            Text = com.Text,
                        });
                    }
                }
                var authorProblem = _context.Users.FirstOrDefault(x => x.Id == current_problem.AuthorId);
                if (current_problem.Images != "") 
                {
                    List<int>? photosId = JsonSerializer.Deserialize<List<int>>(current_problem.Images);
                    return _problemRepository.GetProblemWithPhoto(current_problem, comments, photosId, authorProblem);
                }
                return _problemRepository.GetProblem(current_problem, comments, authorProblem);
            }
            return NotFound();
        }

        [Authorize(Policy = "ProblemCreators")]
        [HttpPut(Name = "AddImagesToProblem")]
        public async Task<ActionResult> AddImagesToProblem(List<IFormFile> _IFormFile, int problemId)
        {
            var current_problem = _context.Problems.FirstOrDefault(x => x.Id == problemId);
            if (current_problem != null)
            {
                var result = await _iManageImage.UploadProblemFiles(_IFormFile, current_problem);
                _unitOfWork.Commit();
                return Ok(result);
            }
            return NotFound();
        }

        [Authorize(Policy = "ProblemCreators")]
        [HttpDelete(Name = "DeleteProblem")]
        public async Task<ActionResult> DeleteProblem(int problemId)
        {
            var current_problem = _context.Problems.FirstOrDefault(x => x.Id == problemId);
            var user = await GetCurrentUserAsync();
            if (current_problem != null && user != null)
            {
                if (current_problem.AuthorId == user.Id)
                {
                    _problemRepository.RemoveProblem(current_problem);
                    _unitOfWork.Commit();
                    return Ok();
                }
                return Forbid();
            }
            return NotFound();
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        
    }
}
