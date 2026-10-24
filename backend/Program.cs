using backend.Data;
using backend.Models;

using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---- Settings (from appsettings.json / appsettings.{Environment}.json) ----
var connectionString = builder.Configuration.GetConnectionString("Default")
  ?? throw new InvalidOperationException("ConnectionStrings:Default is missing.");

var jwtKey = builder.Configuration["Jwt:Key"]
  ?? throw new InvalidOperationException("Jwt:Key is missing.");
if (jwtKey.Length < 32)
  throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// ---- Services ----
builder.Services.AddDbContext<AppDbContext>(options =>
options
      .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
      .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidIssuer = jwtIssuer,

      ValidateAudience = true,
      ValidAudience = jwtAudience,

      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(jwtKey)),

      ValidateLifetime = true
    };
  });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter your JWT token here."
  });

  options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
  {
    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
  });
});

// ---- Pipeline ----
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
var apiV1 = app.MapGroup("/api/v1");

// LOGINNNN
apiV1.mapPost("/login", async (AppDbContext db, IPasswordHasher<User> hasher, LoginRequest request) =>
{
  var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
  if (user == null)
    return Results.Unauthorized();

  var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
  if (result == PasswordVerificationResult.Failed)
    return Results.Unauthorized();

  // Generate JWT token
  var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
  var key = Encoding.UTF8.GetBytes(jwtKey);
  var tokenDescriptor = new SecurityTokenDescriptor
  {
    Subject = new System.Security.Claims.ClaimsIdentity(new[]
    {
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
    }),
    Expires = DateTime.UtcNow.AddHours(1),
    Issuer = jwtIssuer,
    Audience = jwtAudience,
    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
  };
  var token = tokenHandler.CreateToken(tokenDescriptor);
  var jwtToken = tokenHandler.WriteToken(token);

  return Results.Ok(new { Token = jwtToken });
});

// Schedules

apiV1.mapGet("/schedules", async (AppDbContext db) =>
{
  var schedules = await db.ScheduleItems
    .Include(s => s.ScheduleStaff)
    .ThenInclude(ss => ss.Staff)
    .ToListAsync();

  return Results.Ok(schedules);
});


app.Run();