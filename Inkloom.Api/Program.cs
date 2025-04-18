using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Inkloom.Api.Extensions;
using Inkloom.Api.Email;

using Inkloom.Api.Middlewares;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContext<DataContext>(options =>
{
  options.UseNpgsql(builder.Configuration["PgConnectionString"]);
  if (!builder.Environment.IsProduction())
  {
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
  }
});

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

builder.AddEmailService<EmailService>();

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

builder.Services.AddHttpLogging(options =>
{
  options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod |
                          Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
                          Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestQuery |
                          Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode |
                          Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.Duration;
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

app.UseHttpLogging();

app.UseExceptionMiddleware();

app.UseAuthentication();

app.UseTokenBlacklistMiddleware();

app.UseAuthorization();

app.UseCors(InkloomAllowedOrigins);

app.MapControllers();

app.Run();
