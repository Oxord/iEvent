using iEvent.Auth;
using iEvent.Auth.ProblemDto;
using iEvent.Auth.UserDto;
using iEvent.Domain;
using iEvent.Domain.Models;
using iEvent.Domain.Repositories;
using iEvent.DTO;
using iEvent.DTO.UserDto;
using iEvent.Infastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace iEvent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IManageImage _iManageImage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly ICompanyInfoService _service;
        public UserController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager, 
            IConfiguration configuration, ApplicationDbContext context,
            IUnitOfWork unitOfWork, IManageImage iManageImage,
            IUserRepository userRepository, IWebHostEnvironment hostingEnv, ICompanyInfoService service)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _context = context;
            _iManageImage = iManageImage;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _hostingEnv = hostingEnv;
            _service = service;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    //new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Username,
                Type = model.Type,
                Class = model.Class,
                Patronymic = model.Patronymic,
                ProfilePhoto = 0,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            var type = user.Type;
            /*
            if (!await _roleManager.RoleExistsAsync(UserRoles.Student) && type == "Ученик") //!await _roleManager.RoleExistsAsync(UserRoles.Student) &&
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Student);
            }
            */
            if (!await _roleManager.RoleExistsAsync(UserRoles.Teacher))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Teacher));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Parent))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Parent));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Administrator))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Administrator));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Classman))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Classman));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await _roleManager.RoleExistsAsync(UserRoles.Teacher) && type == "Учитель")
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Teacher);
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Student) && type == "Ученик")
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Student);
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Parent) && type == "Родитель")
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Parent);
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Administrator) && type == "Администратор")
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Administrator);
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Classman) && type == "Студент")
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Classman);
            }

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("Users/Current")]
        public async Task<ActionResult<User>> getloggedInUserId()
        {
            var user = await GetCurrentUserAsync();

            return user;
        }
        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [HttpPut("EditUserPhoto")]
        [Authorize]
        public async Task<ActionResult<User>> EditUserPhoto(IFormFile _IFormFile)
        {
            var user = await GetCurrentUserAsync();
            var result = await _iManageImage.UploadUserPhoto(_IFormFile, user);
            _unitOfWork.Commit();
            return Ok(result);
        }


        [HttpPut("EditUser")]
        [Authorize]
        public async Task<ActionResult<User>> EditUser([FromBody] EditUserModel model)
        {
            var user = await GetCurrentUserAsync();
            user.Name = model.Name;
            user.Surname = model.Surname;
            _unitOfWork.Commit();
            return Ok();
        }
        [HttpGet("GetUserPhoto")]
        [Authorize]
        public async Task<IActionResult> GetUserPhoto()
        {
            var user = await GetCurrentUserAsync();
            var currnetPhoto = _context.Photos.FirstOrDefault(x => x.Id == user.ProfilePhoto);
            if (currnetPhoto != null)
            {
                var result = await _iManageImage.DownloadFile(currnetPhoto);
                return File(result.Item1, result.Item2, result.Item2);
            }
            var resul = await _iManageImage.DownloadDefaultUserIcon();
            return File(resul.Item1, resul.Item2, resul.Item2);
        }

        [Authorize]
        [HttpGet("GetUserProblems")]
        public async Task<ActionResult<List<ProblemInList>>> GetUserProblems()
        {
            var user = await GetCurrentUserAsync();
            if (user != null) return _userRepository.GetUserProblems(user);
            return NotFound();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        /*
        [HttpPost("UserPhoto")]
        public async Task<ActionResult<String>> UserPhoto(IFormFile _IFormFile)
        {
            try { await _service.updateUserCompanyInfoWithFile(_IFormFile); } catch (Exception ex) { throw ex; }; 
            var result = await _service.updateUserCompanyInfoWithFile(_IFormFile);
            return result;
        }
        */
    }

    public interface ICompanyInfoService
    {
        Task<string> updateUserCompanyInfoWithFile(IFormFile info);
    }

    internal class CompanyInfoService: ICompanyInfoService
    {

        public static IWebHostEnvironment _environment;
        public CompanyInfoService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> updateUserCompanyInfoWithFile(IFormFile info)
        {
            await CreateFile("./Uploads/Images/", info);
            return "Ok";
        }
        private async Task CreateFile(string savePath, IFormFile formFile)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            if (formFile.Length <= 0) return;

            using (var filestream = File.Create($"{savePath}{formFile.FileName}"))
            {
                await formFile.CopyToAsync(filestream); //Problem is here
            }
        }
    }

}