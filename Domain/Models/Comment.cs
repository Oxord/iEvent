namespace iEvent.Domain.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public int authorImage { get; set; }
        public string authorLogin { get; set; }
        public Event Event { get; set; }
        public string Images { get; set; }
    }
}

