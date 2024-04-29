using iEvent.DTO.CommentDto;

namespace iEvent.DTO.ProblemDto
{
    public class ProblemOnly
    {
        public string Title { get; set; }
        public string DescriptionText { get; set; }
        public string Category { get; set; }
        public string authorName { get; set; }
        public string authorSurname { get; set; }
        public List<ViewCommentModel> Comments { get; set; }
        public List<int> photos { get; set; }
    }
}
