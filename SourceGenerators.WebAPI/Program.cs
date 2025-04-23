using SourceGenerators.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

app.MapGet("/add/{a:int}/{b:int}", (int a, int b) => new Calculator().Add(a, b));
app.MapGet("/subtract/{a:int}/{b:int}", (int a, int b) => new Calculator().Subtract(a, b));
app.MapGet("/multiply/{a:int}/{b:int}", (int a, int b) => new Calculator().Multiply(a, b));
app.MapGet("/divide/{a:int}/{b:int}", (int a, int b) => new Calculator().Divide(a, b));

app.Run();