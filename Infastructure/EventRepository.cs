using iEvent.Auth.EventDto;
using iEvent.Domain.Models;
using iEvent.DTO.CommentDto;
using iEvent.DTO.EventDto;
using iEvent.Infastructure;
using System.Xml.Linq;

namespace iEvent.Domain.Repositories
{
    internal class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddEvent(Event even)
        {
            _context.TableEvents.Add(even);
        }

        public List<EventOnMap> GetEvents(List<Event> events)
        {
            return events.ConvertAll(x => new EventOnMap() { Name = x.Name, Date = x.Date, DescriptionText = x.DescriptionText, Mark = x.Mark });
        }

        public EventOnly GetEventById(Event even, List<ViewCommentModel> comments, List<int> photos)
        {
            if (photos == null)
            {
                return new EventOnly()
                {
                    Name = even.Name,
                    Date = even.Date,
                    DescriptionText = even.DescriptionText,
                    Mark = even.Mark,
                    Comments = comments,

                };
            }
            else
            {
                return new EventOnly()
                {
                    Name = even.Name,
                    Date = even.Date,
                    DescriptionText = even.DescriptionText,
                    Mark = even.Mark,
                    Comments = comments,
                    photos = photos
                };
            }
        }
    }
}
