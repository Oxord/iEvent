using iEvent.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace iEvent.DTO.MapOfEventDto
{
    public class CreateMapModel
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? Audience { get; set; }

    }
}
