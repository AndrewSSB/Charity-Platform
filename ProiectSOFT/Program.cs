using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.BLL.Interfaces;
using ProiectSoft.BLL.Managers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Seeders;
using ProiectSoft.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Connection to database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.AddTransient<Location, Location>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Call Services
builder.Services.AddServices();
builder.Services.AddTransient<ITokenHelper, TokenHelper>();
builder.Services.AddTransient<IAuthManager, AuthManager>();


//Call seeders
builder.Services.AddTransient<CasesSeeder>();
builder.Services.AddTransient<OrganisationSeeder>();
builder.Services.AddTransient<LocationSeeder>();
builder.Services.AddTransient<ShelterSeeder>();
builder.Services.AddTransient<RefugeesSeeder>();
builder.Services.AddTransient<RoleSeeder>();

//Identity

builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("AuthScheme", options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    var secret = builder.Configuration.GetSection("JWT").GetSection("Secret").Get<String>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => policy.RequireRole("Admin").RequireAuthenticatedUser().AddAuthenticationSchemes("AuthScheme").Build());
    opt.AddPolicy("User", policy => policy.RequireRole("User").RequireAuthenticatedUser().AddAuthenticationSchemes("AuthScheme").Build());
});


var app = builder.Build();

//Dupa ce se creeaza aplicatia apelam functia care injecteaza seederele

SeedInjection(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void SeedInjection(IHost app)
{
    using (var scope = app.Services.CreateScope())
    {
        var seedCase = scope.ServiceProvider.GetRequiredService<CasesSeeder>();
        seedCase.Seed();

        var seedOrg = scope.ServiceProvider.GetRequiredService<OrganisationSeeder>();
        seedOrg.Seed();

        var seedLoc = scope.ServiceProvider.GetRequiredService<LocationSeeder>();
        seedLoc.Seed();

        var seedShel = scope.ServiceProvider.GetRequiredService<ShelterSeeder>();
        seedShel.Seed();

        var seedRef = scope.ServiceProvider.GetRequiredService<RefugeesSeeder>();
        seedRef.Seed();

        var seedRole = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
        seedRole.CreateRoles();
    }
}
