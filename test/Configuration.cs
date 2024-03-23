using AutoMapper;
using InkloomApi.Data;
using InkloomApi.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace test
{
    public class TestDataContext(DbContextOptions<DataContext> options, IConfiguration config) : DataContext(options)
    {
        private readonly IConfiguration _config = config;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config["PgConnectionString"]);
        }
    };
    public class Configuration : IDisposable
    {
        public static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").AddEnvironmentVariables().Build();

        public readonly TestDataContext dataContext = new(new(), config);

        public readonly IMapper autoMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new UserProfile());
        }).CreateMapper();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }

    [CollectionDefinition("Database Collection")]
    public class InkloomTestsCollection : ICollectionFixture<Configuration> { }

}


