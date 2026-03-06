using System.Collections.Generic;
using System.Threading.Tasks;
using NMolecules.Analyzers.AggregateRootAnalyzers;
using NMolecules.Analyzers.Test.AggregateRootAnalyzerTests.SampleData;
using Xunit;
using static NMolecules.Analyzers.Test.ElementNames;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.AggregateRootAnalyzers.AggregateRootAnalyzer>;

namespace NMolecules.Analyzers.Test.AggregateRootAnalyzerTests
{
    public class AggregateRootUsesOtherElements
    {
        private const int FieldLineNumber = 14;
        private const int CtorLineNumber = 15;
        private const int PropertyLineNumber = 23;
        private const int MethodLineNumber = 25;
        private const int TypeViolationInMethodBodyLineNumber = 27;

        [Fact]
        public async Task Analyze_WithAggregateRootUsesRepository_EmitsCompilerError()
        {
            var aggregateRoot = GenerateClass(Repository);

            var repositoryAsField = CompilerError(Rules.AggregateRootsShouldNotUseRepositoriesRuleId).WithSpan(FieldLineNumber, 41, FieldLineNumber, 51);
            var repositoryAsParameterInCtor = CompilerError(Rules.AggregateRootsShouldNotUseRepositoriesRuleId).WithSpan(CtorLineNumber, 52, CtorLineNumber, 57);
            var repositoryAsReturnValue = CompilerError(Rules.AggregateRootsShouldNotUseRepositoriesRuleId).WithSpan(MethodLineNumber, 31, MethodLineNumber, 41);
            var repositoryAsParameterInMethod = CompilerError(Rules.AggregateRootsShouldNotUseRepositoriesRuleId).WithSpan(MethodLineNumber, 57, MethodLineNumber, 67);
            var repositoryAsPropertyType = CompilerError(Rules.AggregateRootsShouldNotUseRepositoriesRuleId).WithSpan(PropertyLineNumber, 31, PropertyLineNumber, 36);
            var repositoryInMethodBody = CompilerError(Rules.AggregateRootsShouldNotUseRepositoriesRuleId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 31);
            await VerifyCS.VerifyAnalyzerAsync(aggregateRoot,
                repositoryAsField,
                repositoryAsParameterInCtor,
                repositoryAsParameterInMethod,
                repositoryAsReturnValue,
                repositoryAsPropertyType,
                repositoryInMethodBody);
        }
        
        [Fact]
        public async Task Analyze_WithAggregateRootUsesService_EmitsCompilerError()
        {
            var aggregateRoot = GenerateClass(Service);
            var serviceAsField = CompilerError(Rules.AggregateRootsShouldNotUseLegacyServicesRuleId).WithSpan(FieldLineNumber, 38, FieldLineNumber, 45);
            var serviceAsParameterInCtor = CompilerError(Rules.AggregateRootsShouldNotUseLegacyServicesRuleId).WithSpan(CtorLineNumber, 49, CtorLineNumber, 54);
            var serviceAsReturnValue = CompilerError(Rules.AggregateRootsShouldNotUseLegacyServicesRuleId).WithSpan(MethodLineNumber, 28, MethodLineNumber, 38);
            var serviceAsParameterInMethod = CompilerError(Rules.AggregateRootsShouldNotUseLegacyServicesRuleId).WithSpan(MethodLineNumber, 51, MethodLineNumber, 58);
            var serviceAsPropertyType = CompilerError(Rules.AggregateRootsShouldNotUseLegacyServicesRuleId).WithSpan(PropertyLineNumber, 28, PropertyLineNumber, 33);
            var serviceInMethodBody = CompilerError(Rules.AggregateRootsShouldNotUseLegacyServicesRuleId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 28);
            await VerifyCS.VerifyAnalyzerAsync(aggregateRoot,
                serviceAsField,
                serviceAsParameterInCtor,
                serviceAsParameterInMethod,
                serviceAsReturnValue,
                serviceAsPropertyType,
                serviceInMethodBody);
        }

        [Fact]
        public async Task Analyze_WithAggregateRootUsesDomainService_EmitsCompilerError()
        {
            var aggregateRoot = ServiceRoleShims.AppendIfNeeded(GenerateClass(DomainService), DomainService);
            var domainServiceAsField = CompilerError(Rules.AggregateRootsShouldNotUseServicesRuleId).WithSpan(FieldLineNumber, 44, FieldLineNumber, 57);
            var domainServiceAsParameterInCtor = CompilerError(Rules.AggregateRootsShouldNotUseServicesRuleId).WithSpan(CtorLineNumber, 55, CtorLineNumber, 60);
            var domainServiceAsReturnValue = CompilerError(Rules.AggregateRootsShouldNotUseServicesRuleId).WithSpan(MethodLineNumber, 34, MethodLineNumber, 44);
            var domainServiceAsParameterInMethod = CompilerError(Rules.AggregateRootsShouldNotUseServicesRuleId).WithSpan(MethodLineNumber, 63, MethodLineNumber, 76);
            var domainServiceAsPropertyType = CompilerError(Rules.AggregateRootsShouldNotUseServicesRuleId).WithSpan(PropertyLineNumber, 34, PropertyLineNumber, 39);
            var domainServiceInMethodBody = CompilerError(Rules.AggregateRootsShouldNotUseServicesRuleId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 34);
            await VerifyCS.VerifyAnalyzerAsync(aggregateRoot,
                domainServiceAsField,
                domainServiceAsParameterInCtor,
                domainServiceAsParameterInMethod,
                domainServiceAsReturnValue,
                domainServiceAsPropertyType,
                domainServiceInMethodBody);
        }

        [Fact]
        public async Task Analyze_WithAggregateRootUsesAggregateRoot_EmitsCompilerError()
        {
            var aggregateRoot = @"namespace NMolecules.Analyzers.Test.AggregateRootAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [AggregateRoot]
    public class SomeAggregateRoot
    {
        [Identity]
        public Guid Id { get; }
    }

    [AggregateRoot]
    public sealed class InvalidAggregateRoot
    {
        [Identity]
        public Guid Id { get; }

        private readonly SomeAggregateRoot {|#0:other|};

        public InvalidAggregateRoot(SomeAggregateRoot {|#1:value|})
        {
            Value = value;
        }

        public SomeAggregateRoot {|#2:Value|} { get; set; }

        public SomeAggregateRoot {|#3:SomeMethod|}(SomeAggregateRoot {|#4:input|})
        {
            var {|#5:local|} = new SomeAggregateRoot();
            return local;
        }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(
                aggregateRoot,
                CompilerError(Rules.AggregateRootsShouldNotUseAggregateRootsRuleId).WithLocation(0),
                CompilerError(Rules.AggregateRootsShouldNotUseAggregateRootsRuleId).WithLocation(1),
                CompilerError(Rules.AggregateRootsShouldNotUseAggregateRootsRuleId).WithLocation(2),
                CompilerError(Rules.AggregateRootsShouldNotUseAggregateRootsRuleId).WithLocation(3),
                CompilerError(Rules.AggregateRootsShouldNotUseAggregateRootsRuleId).WithLocation(4),
                CompilerError(Rules.AggregateRootsShouldNotUseAggregateRootsRuleId).WithLocation(5));
        }

        [Fact]
        public async Task Analyze_WithAggregateRootUsesFactory_EmitsCompilerError()
        {
            var aggregateRoot = GenerateClass(Factory);
            var factoryAsField = CompilerError(Rules.AggregateRootsShouldNotUseFactoriesRuleId).WithSpan(FieldLineNumber, 38, FieldLineNumber, 45);
            var factoryAsParameterInCtor = CompilerError(Rules.AggregateRootsShouldNotUseFactoriesRuleId).WithSpan(CtorLineNumber, 49, CtorLineNumber, 54);
            var factoryAsReturnValue = CompilerError(Rules.AggregateRootsShouldNotUseFactoriesRuleId).WithSpan(MethodLineNumber, 28, MethodLineNumber, 38);
            var factoryAsParameterInMethod = CompilerError(Rules.AggregateRootsShouldNotUseFactoriesRuleId).WithSpan(MethodLineNumber, 51, MethodLineNumber, 58);
            var factoryAsPropertyType = CompilerError(Rules.AggregateRootsShouldNotUseFactoriesRuleId).WithSpan(PropertyLineNumber, 28, PropertyLineNumber, 33);
            var factoryInMethodBody = CompilerError(Rules.AggregateRootsShouldNotUseFactoriesRuleId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 28);
            await VerifyCS.VerifyAnalyzerAsync(aggregateRoot,
                factoryAsField,
                factoryAsParameterInCtor,
                factoryAsParameterInMethod,
                factoryAsReturnValue,
                factoryAsPropertyType,
                factoryInMethodBody);
        }

        [Fact]
        public async Task Analyze_WithAggregateRootUsesApplicationService_EmitsCompilerError()
        {
            var aggregateRoot = ServiceRoleShims.AppendIfNeeded(GenerateClass(ApplicationService), ApplicationService);
            var applicationServiceAsField = CompilerError(Rules.AggregateRootsShouldNotUseApplicationServicesRuleId).WithSpan(FieldLineNumber, 49, FieldLineNumber, 67);
            var applicationServiceAsParameterInCtor = CompilerError(Rules.AggregateRootsShouldNotUseApplicationServicesRuleId).WithSpan(CtorLineNumber, 60, CtorLineNumber, 65);
            var applicationServiceAsReturnValue = CompilerError(Rules.AggregateRootsShouldNotUseApplicationServicesRuleId).WithSpan(MethodLineNumber, 39, MethodLineNumber, 49);
            var applicationServiceAsParameterInMethod = CompilerError(Rules.AggregateRootsShouldNotUseApplicationServicesRuleId).WithSpan(MethodLineNumber, 73, MethodLineNumber, 91);
            var applicationServiceAsPropertyType = CompilerError(Rules.AggregateRootsShouldNotUseApplicationServicesRuleId).WithSpan(PropertyLineNumber, 39, PropertyLineNumber, 44);
            var applicationServiceInMethodBody = CompilerError(Rules.AggregateRootsShouldNotUseApplicationServicesRuleId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 39);
            await VerifyCS.VerifyAnalyzerAsync(aggregateRoot,
                applicationServiceAsField,
                applicationServiceAsParameterInCtor,
                applicationServiceAsParameterInMethod,
                applicationServiceAsReturnValue,
                applicationServiceAsPropertyType,
                applicationServiceInMethodBody);
        }
        
        [Fact]
        public async Task Analyze_ValidAggregateRoot_DoesNotEmitAnyError()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("ValidMaximumAggregate.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot);
        }
        
        private static string GenerateClass(string type)
        {
            var invalidUsageTemplate = new InvalidUsageTemplate
            {
                Session = new Dictionary<string, object> { { "type", type }, { "name", type.ToLowerInvariant() } }
            };
            return invalidUsageTemplate.TransformText();
        }
    }
}
