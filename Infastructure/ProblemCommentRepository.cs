using iEvent.Domain.Models;
using iEvent.Infastructure;

namespace iEvent.Domain.Repositories
{

    internal class ProblemCommentRepository : IProblemCommentRepository
    {
        private readonly ApplicationDbContext _context;

        public ProblemCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddComment(ProblemComment com)
        {
            _context.ProblemComments.Add(com);
        }

    }
}
