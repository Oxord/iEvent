namespace iEvent.Domain.Models
{
    public class EventMarkUser
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public bool IsMarked { get; set; }  
    }
}
