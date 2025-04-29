using SourceGenerators.WebAPI;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () =>
{

    Calculator calculator = new Calculator();
    calculator.A = 20;
    calculator.B = 10;

    return calculator.Subtract(calculator.A, calculator.B);

});

app.Run();