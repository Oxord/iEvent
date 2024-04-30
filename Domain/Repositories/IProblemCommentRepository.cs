using iEvent.Domain.Models;

namespace iEvent.Domain.Repositories
{
    public interface IProblemCommentRepository
    {
        void AddComment(ProblemComment com);
    }
}
