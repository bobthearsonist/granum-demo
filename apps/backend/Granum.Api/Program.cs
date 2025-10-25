using Granum.Api.Features.User;
using Granum.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
//TODO add Scalar UI

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("GranumDb")); //TODO use a factory so you can switch between providers for tests and production
builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>)); //TODO use source-generated DI with attributes and Microsoft.Extensions.DependencyInjection.SourceGeneration
builder.Services.AddScoped(typeof(IUserService<>), typeof(UserService<>));
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

using var app = builder.Build();
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
