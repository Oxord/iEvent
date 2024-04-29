﻿namespace iEvent.Domain.Models
{
    public class Problem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string DescriptionText { get; set; }
        public string Category { get; set; }
        public string authorName { get; set; }
        public string authorSurname { get; set; }
        public int authorImage { get; set; }
        public string Images { get; set; }
    }
}
