using Azure.Core;
using iEvent.Auth;
using iEvent.Auth.MapOfEventDto;
using iEvent.Auth.UserDto;
using iEvent.Domain.Models;
using iEvent.DTO;
using iEvent.DTO.MapOfEventDto;
using iEvent.Infastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace iEvent.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MapOfEventController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public MapOfEventController(UserManager<User> userManager, IConfiguration configuration, ApplicationDbContext context)

        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpPost(Name = "CreateMapOfEvent")]
        public async Task<IActionResult> CreateMap([FromBody] CreateMapModel model)
        {
            bool mapExists = false;
            foreach (MapOfEvent map in _context.mapOfEvents)
            {
                if (model.Name == map.Name && model.Description == map.Description && model.Audience == map.Audience)
                {
                    mapExists = true;
                }
            }
            if (mapExists == true)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Map of event already exists!" });
            }

            MapOfEvent mapOfevent = new()
            {
                Name = model.Name,
                Description = model.Description,
                Audience = model.Audience,
            };

            _context.Add(mapOfevent);
            _context.SaveChanges();
            return Ok(new Response { Status = "Success", Message = "Map of Event created successfully!" });

        }


        [Authorize]
        [HttpGet(Name = "GetMaps")]
        public async Task<ActionResult<List<MapOfEventView>>> GetMaps()
        {
            int Events_Count = 0;
            var user = await GetCurrentUserAsync();
            var userClass = user?.Class;
            List<MapOfEvent> events = new List<MapOfEvent>();
            foreach (MapOfEvent map in _context.mapOfEvents)
            {
                if (Convert.ToString(userClass) == map.Audience || map.Audience == "all")
                {
                    Events_Count++;
                    events.Add(map);
                }
            }
            if (Events_Count > 0)
            {
                return events.ToList().ConvertAll(x => new MapOfEventView() { Name = x.Name, Description = x.Description });
            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Для вас карт событий не нашлось :(" });

        }

        [HttpGet]
        public async Task<ActionResult<MapOfEventView>> GetMap(int mapId)
        {
            var CurrentMap = GetMapById(mapId);
            if (CurrentMap != null)
            {
                return new MapOfEventView()
                {
                    Name = CurrentMap.Name,
                    Description = CurrentMap.Description,
                };
            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Такой карты событий нету" });
        }


        private MapOfEvent GetMapById(int mapId)
        {
            MapOfEvent CurrentMap = null;
            foreach (MapOfEvent map in _context.mapOfEvents)
            {
                if (map.Id == mapId)
                {
                    CurrentMap = map;
                    break;
                }
            }
            return CurrentMap;
        }
    }
}
