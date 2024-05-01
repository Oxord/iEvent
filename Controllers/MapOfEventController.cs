using Azure.Core;
using iEvent.Auth;
using iEvent.Auth.MapOfEventDto;
using iEvent.Auth.UserDto;
using iEvent.Domain;
using iEvent.Domain.Models;
using iEvent.Domain.Repositories;
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
        private readonly IMapOfEventRepository _MapRepository;
        private readonly IUnitOfWork _unitOfWork;
        public MapOfEventController(UserManager<User> userManager, IConfiguration configuration, 
            ApplicationDbContext context, IUnitOfWork unitOfWork, 
            IMapOfEventRepository mapRepository)

        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _unitOfWork = unitOfWork;
            _MapRepository = mapRepository;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize(Policy = "CreatorMaps")]
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
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            MapOfEvent mapOfevent = new()
            {
                Name = model.Name,
                Description = model.Description,
                Audience = model.Audience,
            };

            _MapRepository.AddMapOfEvent(mapOfevent);
            _unitOfWork.Commit();
            return Ok();

        }

        [Authorize(Policy = "CreatorMaps")]
        [HttpPut(Name = "EditMapOfEvent")]
        public async Task<IActionResult> EditMapOfEvent([FromBody] CreateMapModel model, int mapId)
        {
            var current_map = _context.mapOfEvents.FirstOrDefault(x => x.Id == mapId);

            if (current_map == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            current_map.Name = model.Name;
            current_map.Description = model.Description;
            current_map.Audience = model.Audience;
            _unitOfWork.Commit();
            return Ok();

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
                return _MapRepository.GetMaps(events);
            }
            return NotFound();

        }

        [HttpGet]
        public async Task<ActionResult<MapOfEventView>> GetMap(int mapId)
        {
            var CurrentMap = GetMapById(mapId);
            if (CurrentMap != null)
            {
                return _MapRepository.GetMap(CurrentMap);
            }
            return NotFound();
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
