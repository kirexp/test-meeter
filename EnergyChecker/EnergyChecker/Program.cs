using EnergyChecker.DataAccess;
using EnergyChecker.DataAccess.Seed;
using EnergyChecker.Services;
using EnergyChecker.Utils;
using EnergyChecker.Utils.Parsers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProcessReadingsService, ProcessReadingsService>();
builder.Services.AddSingleton<ParserFactory>();

builder.Services.AddDbContextPool<IApplicationDbContext, ApplicationDbContext>((provider, options) =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypass");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // or "http://localhost:3000" if using CRA
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowReactFrontend");

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
if (!context.Accounts.Any())
{
    AccountSeeder.Seed("./TestData/Test_Accounts.ods", context);
    await context.SaveChangesAsync(CancellationToken.None);
}

scope.Dispose();


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