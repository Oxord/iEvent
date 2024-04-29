﻿namespace iEvent.Domain.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string authorName { get; set; }
        public string authorSurname { get; set; }
        public int authorImage { get; set; }
        public Event Event { get; set; }
        public string Images { get; set; }
    }
}

