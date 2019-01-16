using Infrastructure.Interfaces.DBConfiguration;

namespace Infrastructure.DBConfiguration.Dapper
{
    public class DataSettings : IDataSettings
    {
        public string DefaultConnection { get; set; }
    }
}
