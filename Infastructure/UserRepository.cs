using iEvent.Auth.ProblemDto;
using iEvent.Domain.Models;
using iEvent.Infastructure;

namespace iEvent.Domain.Repositories
{
    internal class UserRepository: IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<ProblemInList> GetUserProblems(User currentUser)
        {
            return _context.Problems.Where(x => x.AuthorId == currentUser.Id).ToList().ConvertAll(x => new ProblemInList { Title = x.Title, Category = x.Category });
        }
    }
}
