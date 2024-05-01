namespace iEvent.Domain.Models
{
    public class ProblemComment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public int authorImage { get; set; }
        public string authorLogin { get; set; }
        public Problem Problem { get; set; }
        public string Images { get; set; } 
    }
}
