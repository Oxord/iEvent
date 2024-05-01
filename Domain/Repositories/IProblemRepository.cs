using iEvent.Auth.MapOfEventDto;
using iEvent.Auth.ProblemDto;
using iEvent.Domain.Models;
using iEvent.DTO.CommentDto;
using iEvent.DTO.ProblemDto;

namespace iEvent.Domain.Repositories
{
    public interface IProblemRepository
    {
        void AddProblem(Problem problem);
        List<ProblemInList> GetProblems();
        ProblemOnly GetProblemWithPhoto(Problem current_problem, List<ViewCommentModel> comments, List<int> photos, User authorProblem);
        ProblemOnly GetProblem(Problem current_problem, List<ViewCommentModel> comments, User authorProblem);
        void RemoveProblem(Problem current_problem);

        //MapOfEventView GetMap(MapOfEvent map);
    }
}
