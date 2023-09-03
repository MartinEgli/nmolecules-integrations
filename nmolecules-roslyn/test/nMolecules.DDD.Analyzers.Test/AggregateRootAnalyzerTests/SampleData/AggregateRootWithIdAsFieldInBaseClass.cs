namespace NMolecules.DDD.Analyzers.Test.AggregateRootAnalyzerTests.SampleData;

[AggregateRoot]
public class AggregateRootWithIdAsField : AggregateRootWithIdAsFieldBase
{
}

public class AggregateRootWithIdAsFieldBase
{
    [Identity] public string Id = "SomeId";
}