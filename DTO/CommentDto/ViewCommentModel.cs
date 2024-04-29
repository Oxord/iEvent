using iEvent.Domain.Models;

namespace iEvent.DTO.CommentDto
{
    public class ViewCommentModel
    {
        public string Text { get; set; }
        public string authorName { get; set; }
        public string authorSurname { get; set; }
        public int authorAvatar {  get; set; }
        public List<int> photos { get; set; }
    }
}
