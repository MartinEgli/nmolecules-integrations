namespace NMolecules.DDD.Analyzers.Test.AggregateRootAnalyzerTests.SampleData;

public partial class AggregateRootWithIdAsPropertyInPartial
{
    // SomeDomainLogic
}

[AggregateRoot]
public partial class AggregateRootWithIdAsPropertyInPartial
{
    [Identity] public string Id { get; } = "SomeId";
}