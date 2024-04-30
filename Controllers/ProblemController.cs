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



        [Authorize]
        [HttpPost(Name = "CreateProblem")]
        public async Task<ActionResult> CreateProblem(CreateProblemModel model)
        {
            var user = await GetCurrentUserAsync();
            Problem problem = new()
            {
                Title = model.Title,
                DescriptionText = model.DescriptionText,
                Category = model.Category,
                authorName = user.Name,
                authorSurname = user.Surname,
                authorImage = user.ProfilePhoto,
                Images = "",
            };
            _problemRepository.AddProblem(problem);
            _unitOfWork.Commit();
            return Ok();

        }

        //[Authorize]
        [HttpGet(Name = "GetAllProblems")]
        public async Task<ActionResult<List<ProblemInList>>> GetAllProblems()
        {
            return _problemRepository.GetProblems();
        }

        //[Authorize]
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
                    List<int>? com_photosId = new();
                    if (com.Images != "") 
                    { 
                        com_photosId = JsonSerializer.Deserialize<List<int>>(com.Images);
                        comments.Add(new ViewCommentModel
                        {
                            authorName = com.authorName,
                            authorSurname = com.authorSurname,
                            authorAvatar = com.authorImage,
                            Text = com.Text,
                            photos = com_photosId,
                        });
                    }
                    else
                    {
                        comments.Add(new ViewCommentModel
                        {
                            authorName = com.authorName,
                            authorSurname = com.authorSurname,
                            authorAvatar = com.authorImage,
                            Text = com.Text,
                        });
                    }
                }
                if (current_problem.Images != "") 
                {
                    List<int>? photosId = JsonSerializer.Deserialize<List<int>>(current_problem.Images);
                    return _problemRepository.GetProblemWithPhoto(current_problem, comments, photosId);
                }
                return _problemRepository.GetProblem(current_problem, comments);
            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Такой проблемы не найдено" });
        }

        //[Authorize]
        [HttpPut("AddImagesToProblem")]
        public async Task<ActionResult> AddImagesToProblem(List<IFormFile> _IFormFile, int problemId)
        {
            var current_problem = _context.Problems.FirstOrDefault(x => x.Id == problemId);
            if (current_problem != null)
            {
                var result = await _iManageImage.UploadProblemFiles(_IFormFile, current_problem);
                _unitOfWork.Commit();
                return Ok(result);
            }
            return BadRequest("Такой проблемы нету");
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        
    }
}
