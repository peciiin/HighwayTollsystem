using FluentValidation;
using FluentValidation.AspNetCore;
using HighwayTollsystem.Data;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Middlewares;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using HighwayTollsystem.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<HighwayTollContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterNewVehicleDtoValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITollService, TollService>();
builder.Services.AddScoped<IVehicleInspectionService, VehicleInspectionService>();
builder.Services.AddScoped<ISpeedService, SpeedService>();
builder.Services.AddScoped<IVignetteService, VignetteService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();



builder.Services.AddHostedService<TollSimulatorService>();



var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<HighwayTollContext>();

        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
