using System.ComponentModel.DataAnnotations;
using iEvent.Domain.Models;

namespace iEvent.DTO.UserDto
{
    public class RegisterModel
    {
        //[Required(ErrorMessage = "User Name is required")]
        public string? Name { get; set; }

        //[Required(ErrorMessage = "User Name is required")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }

        public int Class { get; set; }
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}