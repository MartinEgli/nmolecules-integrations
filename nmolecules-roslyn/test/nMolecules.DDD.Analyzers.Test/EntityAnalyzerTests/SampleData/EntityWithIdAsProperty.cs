namespace NMolecules.DDD.Analyzers.Test.EntityAnalyzerTests.SampleData;

[Entity]
public class EntityWithIdAsProperty
{
    [Identity] public string Id { get; } = "SomeId";
}