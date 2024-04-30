using iEvent.Auth.MapOfEventDto;
using iEvent.Domain.Models;

namespace iEvent.Domain.Repositories
{
    public interface IMapOfEventRepository
    {
        void AddMapOfEvent(MapOfEvent map);
        List<MapOfEventView> GetMaps(List<MapOfEvent> maps);
        MapOfEventView GetMap(MapOfEvent map);
    }
}
