using iEvent.Domain.Models;

namespace iEvent.Domain.Repositories
{
    public interface ICommentRepository
    {
        void AddComment(Comment com);
    }
}
