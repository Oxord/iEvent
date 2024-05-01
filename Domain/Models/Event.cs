using System.ComponentModel.DataAnnotations;

namespace iEvent.Domain.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public int Mark { get; set; }
        public int WholeMark { get; set; }
        public int MarkCount { get; set; }
        public string DescriptionText { get; set; }
        public MapOfEvent Map { get; set; }
        public List<Comment> Comments { get; set; } 
        public string Images { get; set; }

    }
}
