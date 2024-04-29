namespace iEvent.Domain.Models
{
    public class ProblemComment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string authorName { get; set; }
        public string authorSurname { get; set; }
        public int authorImage { get; set; }
        public Problem Problem { get; set; }
        public string Images { get; set; } 
    }
}
