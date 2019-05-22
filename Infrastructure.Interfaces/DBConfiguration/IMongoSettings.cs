namespace Infrastructure.Interfaces.DBConfiguration
{
    public interface IMongoSettings : IDataSettings
    {
        string DBName { get; set; }
    }
}
