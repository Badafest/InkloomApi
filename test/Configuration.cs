using AutoMapper;
using InkloomApi.Data;
using InkloomApi.Models;
using InkloomApi.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]
[assembly: TestCollectionOrderer(ordererTypeName: "test.TestCollectionOrderer", ordererAssemblyName: "test")]

namespace test
{
    public class TestDataContext(DbContextOptions<DataContext> options, IConfiguration config) : DataContext(options)
    {
        private readonly IConfiguration _config = config;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_config["PgConnectionString"]);
    };
    public class Configuration
    {
        public static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").AddEnvironmentVariables().Build();

        public readonly TestDataContext dataContext = new(new(), config);

        public readonly IMapper autoMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new UserProfile());
        }).CreateMapper();
        public static readonly User validUser = new()
        {
            // A valid username can contain lowercase letters and numbers only
            Username = "test123",
            // A valid password is at least 8 characters long and contains at least 1 uppercase, 1 lowercase and 1 number each
            Password = "Str0ngPassword",
            Email = "test@mail.com"
        };
    }

}


