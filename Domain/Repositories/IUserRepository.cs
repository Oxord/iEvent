using iEvent.Auth.ProblemDto;
using iEvent.Domain.Models;
using iEvent.DTO.ProblemDto;

namespace iEvent.Domain.Repositories
{
    public interface IUserRepository
    {
        List<ProblemInList> GetUserProblems(User currentUser);  
    }
}
