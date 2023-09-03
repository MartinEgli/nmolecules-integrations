namespace NMolecules.DDD.Analyzers.Test.EntityAnalyzerTests.SampleData;

[Entity]
public class EntityWithIdAsPropertyInBase : EntityWithIdAsPropertyBase
{
    [Identity] public string Id { get; } = "SomeId";
}

public class EntityWithIdAsPropertyBase
{
    [Identity] public string Id { get; } = "SomeId";
}