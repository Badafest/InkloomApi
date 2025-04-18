using Inkloom.Api.Data;
using Inkloom.Api.Email;
using Inkloom.Api.Extensions;
using Inkloom.Api.Profiles;
using Inkloom.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inkloom.Api.Test;

public class Startup
{
    public static readonly IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(config);
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<BlogProfile>();
        });

        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(config["PgConnectionString"]);
        }, ServiceLifetime.Transient);

        services.AddSingleton<IEmailService, EmailService>().ConfigureSmtpOptions<EmailService>(config);
        services.AddSingleton<ITokenService, TokenService>();

        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITagService, TagService>();
        services.AddTransient<IBlogService, BlogService>();
    }
}