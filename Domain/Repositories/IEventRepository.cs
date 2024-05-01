using iEvent.Auth.EventDto;
using iEvent.Domain.Models;
using iEvent.DTO.CommentDto;
using iEvent.DTO.EventDto;

namespace iEvent.Domain.Repositories
{
    public interface IEventRepository
    {
        void AddEvent(Event even);
        List<EventOnMap> GetEvents(List<Event> events);
        EventOnly GetEventById(Event even, List<ViewCommentModel> comments, List<int> photos);
    }
}
