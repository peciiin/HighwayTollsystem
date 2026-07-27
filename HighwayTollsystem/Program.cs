using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<HighwayTollContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<HighwayTollsystem.Services.TollService>();
builder.Services.AddScoped<HighwayTollsystem.Services.StkService>();
builder.Services.AddScoped<HighwayTollsystem.Services.SpeedService>();
builder.Services.AddScoped<HighwayTollsystem.Services.VignetteService>();
builder.Services.AddScoped<HighwayTollsystem.Services.AnalyticsService>();
builder.Services.AddScoped<HighwayTollsystem.Services.TollSimulatorService>();

var app = builder.Build();

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
