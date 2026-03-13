using System.Reflection;
using NetArchTest.Rules;

namespace ItransitionCourseProject.ArchitectureTests;

public class DomainModelsDesignTests
{
    private const string DomainModelsNamespace = "ItransitionCourseProject.Api.Models";
    private readonly Assembly _assembly = typeof(Api.AssemblyReference).Assembly;
    
    [Fact]
    public void Domain_Models_Should_Be_Sealed_Classes()
    {
        var result = Types
            .InAssembly(_assembly)
            .That()
            .AreClasses()
            .And()
            .ResideInNamespace(DomainModelsNamespace)
            .Should()
            .BeSealed()
            .GetResult();
        
        var failingTypes = (result.FailingTypes ?? Enumerable.Empty<Type>())
            .Select(t => t.FullName);
        
        Assert.True(result.IsSuccessful, string.Join(Environment.NewLine, failingTypes));
    }
}