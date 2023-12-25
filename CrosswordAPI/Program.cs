using CrosswordGenerator.GenerationManager;
using CrosswordGenerator.Generator;
using CrosswordGenerator.Generator.Interfaces;
using CrosswordGenerator.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenerationManager, GenerationManager>();
builder.Services.AddScoped<IGenerator, Generator>();
builder.Services.AddScoped<IPlacementFinder, PlacementFinder>();
builder.Services.AddScoped<IPuzzleMapper, PuzzleMapper>();


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
