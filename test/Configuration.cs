using AutoMapper;
using InkloomApi.Data;
using InkloomApi.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace test
{
    public class TestDataContext : DataContext
    {
        private readonly IConfiguration _config;
        public TestDataContext(DbContextOptions<DataContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_config["PgConnectionString"]);
    };
    public static class Configuration
    {
        public static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").AddEnvironmentVariables().Build();

        public static readonly TestDataContext dataContext = new(new(), config);

        public static readonly IMapper autoMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new UserProfile());
        }).CreateMapper();

    }
}


