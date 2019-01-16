using System.Data;

namespace Infrastructure.Interfaces.DBConfiguration
{
    public interface IDatabaseFactory
    {
        IDbConnection GetDbConnection { get; }
    }
}
