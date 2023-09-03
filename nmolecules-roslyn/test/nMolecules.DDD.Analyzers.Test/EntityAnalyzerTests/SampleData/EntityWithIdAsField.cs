namespace NMolecules.DDD.Analyzers.Test.EntityAnalyzerTests.SampleData;

[Entity]
public class EntityWithIdAsField
{
    [Identity] public string Id = "SomeId";
}