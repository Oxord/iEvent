using Azure.Core;
using iEvent.Auth;
using iEvent.DTO.CommentDto;
using iEvent.Auth.EventDto;
using iEvent.Auth.UserDto;
using iEvent.Domain.Models;
using iEvent.Infastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using iEvent.DTO;
using iEvent.DTO.EventDto;
using iEvent.Domain;
using System.Text.Json;
using iEvent.Domain.Repositories;

namespace iEvent.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IManageImage _iManageImage;
        private readonly IEventRepository _EventRepository;
        private readonly IUnitOfWork _unitOfWork;
        public EventController(UserManager<User> userManager, IConfiguration configuration, 
            ApplicationDbContext context, IManageImage iManageImage,
            IUnitOfWork unitOfWork, IEventRepository eventRepository)

        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _iManageImage = iManageImage;
            _EventRepository = eventRepository;
            _unitOfWork = unitOfWork;
        }



        [Authorize(Roles = UserRoles.Teacher)]
        [HttpPost(Name = "CreateEvent")]
        public async Task<ActionResult<MapOfEvent>> CreateEvent([FromBody] CreateEventModel model, int mapId)
        {
            //var user = await GetCurrentUserAsync();
            var CurrentMap = GetMapById(mapId);
            if (CurrentMap != null)
            {
                Event even = new()
                {
                    Name = model.Name,
                    Date = model.Date,
                    DescriptionText = model.DescriptionText,
                    Map = CurrentMap,
                    Images = ""
                };
                _EventRepository.AddEvent(even);
                _unitOfWork.Commit();
                return Ok();
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Unlucky" });
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpPut("AddImagesToEvent")]
        public async Task<ActionResult> AddImagesToEvent(List<IFormFile> _IFormFile, int eventId)
        {
            var currentevent = _context.TableEvents.FirstOrDefault(x => x.Id == eventId);
            if (currentevent != null)
            {
                var result = await _iManageImage.UploadFiles(_IFormFile, currentevent);
                _unitOfWork.Commit();
                return Ok(result);
            }
            return BadRequest("Такого события нету");
        }


        [Authorize]
        [HttpGet(Name = "GetEvents")]
        public async Task<ActionResult<List<EventOnMap>>> GetEvents(int mapId)
        {
            var CurrentMap = GetMapById(mapId);
            var resultEvents = new List<Event>();
            if (CurrentMap != null)
            {
                foreach (Event ev in _context.TableEvents)
                {
                    if (ev.Map == CurrentMap)
                    {
                        resultEvents.Add(ev);
                    }
                }
                return _EventRepository.GetEvents(resultEvents);
            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "На этой карте событий не нашлось :(" });
        }

        [Authorize]
        [HttpGet(Name = "GetEventById")]
        public async Task<ActionResult<EventOnly>> GetEvent(int EventId)
        {
            var ev = GetEventById(EventId);
            var resultComments = new List<Comment>();

            if (ev != null)
            {
                foreach (Comment com in _context.Comments.ToList().Where(x => x.Event == ev))
                {
                    resultComments.Add(com);
                }
                List<ViewCommentModel> comments = new();
                foreach (var com in resultComments)
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
                            //photos = ,
                        });
                    }
                }
                return _EventRepository.GetEventById(ev, comments);

            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Такого события нету" });
        }
        
        
        [Authorize]
        [HttpPut(Name = "AddMark")]
        public async Task<ActionResult<Event>> AddMark([FromBody] AddMarkModel model, int EventId, int mapId)
        {
            var user = await GetCurrentUserAsync();
            var CurrentMap = GetMapById(mapId);
            int marks_count = 0;
            if (CurrentMap != null)
            {
                Event ev = _context.TableEvents.FirstOrDefault(x => x.Id == EventId && x.Map == CurrentMap);
                if (ev != null)
                {
                    marks_count++;
                    ev.Mark = (ev.Mark + model.Mark) / marks_count;
                }
                _context.Update(ev);
                _unitOfWork.Commit();
                return ev;
            }

            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Unlucky" });
        }


        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

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
    }
}
