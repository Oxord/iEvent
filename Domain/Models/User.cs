using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace iEvent.Domain.Models
{
    public class User : IdentityUser
    {
        internal string userName;

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Type { get; set; }
        public int Class { get; set; }
        public string Patronymic { get; set; }
        public int ProfilePhoto { get; set; }
        public List<ProblemComment> ProblemComments { get; set; }
    }
}
