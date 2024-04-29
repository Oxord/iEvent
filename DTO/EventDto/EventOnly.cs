using iEvent.DTO.CommentDto;
using iEvent.Domain.Models;

namespace iEvent.DTO.EventDto
{
    public class EventOnly
    {
        public string Name { get; set; } = null;
        public string Date { get; set; } = null;
        public float Mark { get; set; }
        public string DescriptionText { get; set; }
        public List<ViewCommentModel> Comments { get; set; }
        public List<int> photos { get; set; }
    }
}
