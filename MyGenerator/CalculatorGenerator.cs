using Microsoft.CodeAnalysis.Text;

namespace MyGenerator;

[Generator]
public class CalculatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var calculatorClassesProvider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, cancelToken) => 
            node is ClassDeclarationSyntax classDeclaration && 
            classDeclaration.Identifier.ToString() == "Calculator",
            transform: (ctx, cancelToken) => (ClassDeclarationSyntax)ctx.Node);

        context.RegisterSourceOutput(calculatorClassesProvider, 
            (sourceProductionContext, calculatorClass) => Execute(calculatorClass, sourceProductionContext));
    }

    public void Execute(ClassDeclarationSyntax calculatorClass, SourceProductionContext context)
    {
        var calculatorClassMembers = calculatorClass.Members;

        var addMethod = calculatorClassMembers
            .FirstOrDefault(member => member is MethodDeclarationSyntax method && method.Identifier.Text == "Add");
        var subtractMethod = calculatorClassMembers.FirstOrDefault(member => member is MethodDeclarationSyntax method && method.Identifier.Text == "Subtract");
        var multiplyMethod = calculatorClassMembers.FirstOrDefault(member => member is MethodDeclarationSyntax method && method.Identifier.Text == "Multiply");
        var divideMethod = calculatorClassMembers.FirstOrDefault(member => member is MethodDeclarationSyntax method && method.Identifier.Text == "Divide");

        StringBuilder calcGeneratedClassBuilder = new();
        foreach (var usingStatement in calculatorClass.SyntaxTree.GetCompilationUnitRoot().Usings)
        {
            calcGeneratedClassBuilder.AppendLine(usingStatement.ToString());
        }

        calcGeneratedClassBuilder.AppendLine();

        BaseNamespaceDeclarationSyntax? calcClassNamespace = 
            calculatorClass.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        calcClassNamespace ??= calculatorClass.Ancestors()
            .OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault();
        //????
        calcGeneratedClassBuilder.AppendLine($"namespace {calcClassNamespace?.Name};");
        calcGeneratedClassBuilder.AppendLine($"{calculatorClass.Modifiers} class {calculatorClass.Identifier}");
        calcGeneratedClassBuilder.AppendLine("{");

        if (addMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
            """
                    public int Add(int a, int b)
                    {
                        var result = a + b;
                        Console.WriteLine($"The result of adding {a} and {b} is {result}");
                        return result;
                    }
                """);
        }
        if (subtractMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
            """
                
                    public int Subtract(int a, int b)
                    {
                        var result = a - b;
                        if(result < 0)
                        {
                            Console.WriteLine("Result of subtraction is negative");
                        }
                        return result; 
                    }
                """);
        }
        if (multiplyMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
            """
                
                    public int Multiply(int a, int b)
                    {
                        return a * b;
                    }
                """);
        }
        if (divideMethod is null)
        {
            calcGeneratedClassBuilder.AppendLine(
            """

                    public int Divide(int a, int b)
                    {
                        if(b == 0)
                        {
                            throw new DivideByZeroException();
                        }
                        return a / b;
                    }
                """);
        }
        calcGeneratedClassBuilder.AppendLine("}");

        context.AddSource("Calculator.g.cs", SourceText.From(calcGeneratedClassBuilder.ToString(), Encoding.UTF8));
    }
}