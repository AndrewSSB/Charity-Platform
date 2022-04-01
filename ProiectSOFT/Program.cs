using Microsoft.EntityFrameworkCore;
using ProiectSoft.DAL;
using ProiectSoft.DAL.Entities;
using ProiectSoft.DAL.Seeders;
using ProiectSoft.Services;

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

//Call seeders
builder.Services.AddTransient<CasesSeeder>();
builder.Services.AddTransient<OrganisationSeeder>();
builder.Services.AddTransient<LocationSeeder>();
builder.Services.AddTransient<ShelterSeeder>();
builder.Services.AddTransient<RefugeesSeeder>();

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
    }
}
