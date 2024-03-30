using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

using InkloomApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContext<DataContext>(options =>
{
  options.UseNpgsql(builder.Configuration["PgConnectionString"]);
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
      };
    });

builder.Services.AddAuthorizationBuilder().AddPolicy("EmailVerified", policy => policy.RequireClaim("email_verified", "true"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

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

app.UseExceptionMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
