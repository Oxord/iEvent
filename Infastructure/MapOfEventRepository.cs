using iEvent.Auth.MapOfEventDto;
using iEvent.Domain.Models;
using iEvent.Infastructure;

namespace iEvent.Domain.Repositories
{
    internal class MapOfEventRepository: IMapOfEventRepository
    {
        private readonly ApplicationDbContext _context;

        public MapOfEventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddMapOfEvent(MapOfEvent map)
        {
            _context.mapOfEvents.Add(map);
        }
        public List<MapOfEventView> GetMaps(List<MapOfEvent> maps)
        {
            return maps.ToList().ConvertAll(x => new MapOfEventView() { Name = x.Name, Description = x.Description });
        }

        public MapOfEventView GetMap(MapOfEvent map)
        {
            return new MapOfEventView()
            {
                Name = map.Name,
                Description = map.Description,
            };
        }

    }
}
