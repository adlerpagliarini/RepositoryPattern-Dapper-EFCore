using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Repositories.Standard
{
    public interface IRepositoryTransaction
    {
        void StartTransaction();
        Task AbortTransactionAsync();
        Task CommitTransactionAsync();
    }
}
