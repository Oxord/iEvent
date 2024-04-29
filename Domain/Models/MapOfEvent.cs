using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace iEvent.Domain.Models
{
    public class MapOfEvent
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Audience { get; set; }
        public List<Event> Events { get; set; }
    }
}

