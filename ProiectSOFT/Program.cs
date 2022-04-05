using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProiectSoft.BLL.Helpers;
using ProiectSoft.BLL.Interfaces;
using ProiectSoft.BLL.Managers;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Loggers;
using ProiectSoft.DAL.Mappings;
using ProiectSoft.DAL.Seeders;
using ProiectSoft.Services;
using ProiectSoft.Services.EmailService;
using ProiectSoft.Services.EmailServices;
using ProiectSoft.Services.UriServices;
using ProiectSoft.Services.UriServicess;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Connection to database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        B => B.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IUriServices>(o =>
{
    var accesor = o.GetRequiredService<IHttpContextAccessor>();
    var request = accesor.HttpContext.Request;
    var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
    return new UriServices(uri);
});

builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
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
    opt.AddPolicy("Organisation", policy => policy.RequireRole("Organisation").RequireAuthenticatedUser().AddAuthenticationSchemes("AuthScheme").Build());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Authorize for swagger

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ProiectSOFT", Version = "v1"});
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    { 
        {
            new OpenApiSecurityScheme 
            {
                Reference = new OpenApiReference
                { 
                    Type = ReferenceType.SecurityScheme, Id = "Bearer"
                },
            },
            new string[]{}
        }
    });
});

//Call Services
builder.Services.AddServices();
builder.Services.AddTransient<ITokenHelper, TokenHelper>(); //probleme cu ele in extensie, le las aici
builder.Services.AddTransient<IAuthManager, AuthManager>();
builder.Services.AddTransient<IEmailServices, EmailServices>();


//Call seeders
builder.Services.AddTransient<CasesSeeder>();
builder.Services.AddTransient<OrganisationSeeder>();
builder.Services.AddTransient<LocationSeeder>();
builder.Services.AddTransient<ShelterSeeder>();
builder.Services.AddTransient<RefugeesSeeder>();
builder.Services.AddTransient<RoleSeeder>();

//Add mappers
builder.Services.AddMappings();

//UserLog
builder.Host.UseSerilog((ctx, config) =>
{
    /*                config.Enrich.WithCorrelationId();*/
    config.Enrich.FromLogContext();
    config.ReadFrom.Configuration(builder.Configuration);
    config.WriteTo.Console(new LogFormatter());
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

app.UseAuthentication();
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
