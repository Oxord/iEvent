using iEvent.Auth.MapOfEventDto;
using iEvent.Auth.ProblemDto;
using iEvent.Domain.Models;
using iEvent.Domain.Repositories;
using iEvent.DTO.CommentDto;
using iEvent.DTO.ProblemDto;
using System.Xml.Linq;

namespace iEvent.Infastructure
{
    internal class ProblemRepository : IProblemRepository
    {
        private readonly ApplicationDbContext _context;

        public ProblemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddProblem(Problem problem)
        {
            _context.Problems.Add(problem);
        }
        
        public List<ProblemInList> GetProblems()
        {
            return _context.Problems.ToList().ConvertAll(x => new ProblemInList { Title = x.Title, Category = x.Category });
        }

        public ProblemOnly GetProblemWithPhoto(Problem current_problem, List<ViewCommentModel> comments, List<int> photos)
        {
            return new ProblemOnly
            {
                Title = current_problem.Title,
                Category = current_problem.Category,
                DescriptionText = current_problem.DescriptionText,
                authorName = current_problem.authorName,
                authorSurname = current_problem.authorSurname,
                Comments = comments,
                photos = photos,
            };
        }

        public ProblemOnly GetProblem(Problem current_problem, List<ViewCommentModel> comments)
        {
            return new ProblemOnly
            {
                Title = current_problem.Title,
                Category = current_problem.Category,
                DescriptionText = current_problem.DescriptionText,
                authorName = current_problem.authorName,
                authorSurname = current_problem.authorSurname,
                Comments = comments
            };
        }

        /*
        public MapOfEventView GetMap(MapOfEvent map)
        {
            return new MapOfEventView()
            {
                Name = map.Name,
                Description = map.Description,
            };
        }
        */
    }
}
