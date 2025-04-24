using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Inkloom.Api.Extensions;
using Inkloom.Api.Email;

using Inkloom.Api.Middlewares;
using System.Text.Json.Serialization;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

string dbConnectionString = builder.Configuration["PgConnectionString"] ?? "";


builder.Services.AddDbContext<DataContext>(options =>
{
  options.UseNpgsql(dbConnectionString);
  if (!builder.Environment.IsProduction())
  {
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
  }
}).AddSqlDbRecordManager(new()
{
  DbProviderFactory = NpgsqlFactory.Instance, // use Npgsql as db factory,
  ConnectionString = dbConnectionString
}).AddFileAssetManager(new()
{
  BaseDirectory = builder.Configuration["AssetsBaseDirectory"] ?? "",
  GroupByAssetType = true
});

// Run database migration
if (bool.TryParse(builder.Configuration["MigrateDatabase"], out bool migrateDb) && migrateDb)
{
  var dbOptionsBuilder = new DbContextOptionsBuilder<DataContext>();
  dbOptionsBuilder.UseNpgsql(dbConnectionString);
  new DataContext(dbOptionsBuilder.Options).Database.Migrate();
}

builder.Services.AddAuthentication(AUTH_SCHEME)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
      };
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationBuilder().AddPolicy("EmailVerified", policy => policy.RequireClaim("email_verified", "true"));

builder.Services.AddEmailService<EmailService>(builder.Configuration);

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IBlogService, BlogService>();

const string InkloomAllowedOrigins = "_inkloomAllowedOrigins";
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: InkloomAllowedOrigins,
      policy =>
      {
        policy.WithOrigins(builder.Configuration["WebBaseUrl"] ?? "") // Allowed origins
              .AllowAnyMethod()
              .AllowAnyHeader();
      });
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition(AUTH_SCHEME, new OpenApiSecurityScheme
  {
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = AUTH_SCHEME
  });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(InkloomAllowedOrigins);

if (app.Environment.IsProduction())
{
  app.UseHsts();
  app.UseHttpsRedirection();
}
else
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.UseAuthentication();

app.UseTokenBlacklistMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
