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



        [Authorize(Policy = "CreatorMaps")]
        [HttpPost(Name = "CreateEvent")]
        public async Task<ActionResult<MapOfEvent>> CreateEvent([FromBody] CreateEventModel model, int mapId)
        {
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

            return Forbid();
        }

        
        [Authorize(Policy = "CreatorMaps")]
        [HttpPut]
        public async Task<ActionResult> EditEvent(int eventId, [FromBody] CreateEventModel model)
        {
            var currentevent = _context.TableEvents.FirstOrDefault(x => x.Id == eventId);
            if (currentevent != null)
            {
                currentevent.Name = model.Name;
                currentevent.Date = model.Date;
                currentevent.DescriptionText = model.DescriptionText;
                _unitOfWork.Commit();
                return Ok();
            }
            return NotFound();
        }

        [Authorize(Policy = "CreatorMaps")]
        [HttpPut]
        public async Task<ActionResult> AddImagesToEvent(List<IFormFile> _IFormFile, int eventId)
        {
            var currentevent = _context.TableEvents.FirstOrDefault(x => x.Id == eventId);
            if (currentevent != null)
            {
                var result = await _iManageImage.UploadFiles(_IFormFile, currentevent);
                _unitOfWork.Commit();
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet(Name = "GetAllEvents")]
        public async Task<ActionResult<List<EventOnMap>>> GetAllEvents(int mapId)
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
            return NotFound();
        }

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
                List<int> event_photos = new();
                if (ev.Images == "")
                {
                    event_photos = null;
                }
                else
                {
                    event_photos = JsonSerializer.Deserialize<List<int>>(ev.Images);
                }
                return _EventRepository.GetEventById(ev, comments, event_photos);

            }
            return NotFound();
        }
        
        
        [Authorize(Policy = "PeopleCanMark")]
        [HttpPut(Name = "AddMark")]
        public async Task<ActionResult<Event>> AddMark([FromBody] AddMarkModel model, int EventId)
        {
            var user = await GetCurrentUserAsync();
            Event ev = _context.TableEvents.FirstOrDefault(x => x.Id == EventId);
            
            if (ev != null && user != null)
            {
                var eventmarkuser = _context.EventMarkUsers.FirstOrDefault(x => x.EventId == ev.Id && x.UserId == user.Id);
                if (eventmarkuser != null && eventmarkuser.IsMarked)
                {
                    return BadRequest(new Response { Status = "Unluck", Message = "Вы уже оставляли оценку" });
                }
                ev.MarkCount++;
                ev.WholeMark = ev.WholeMark + model.Mark;
                ev.Mark = ev.WholeMark / ev.MarkCount;
                _context.Update(ev);
                _context.EventMarkUsers.Add(new EventMarkUser { EventId = ev.Id, UserId = user.Id, IsMarked = true });
                _unitOfWork.Commit();
                return Ok();
            }
            return Forbid();
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
