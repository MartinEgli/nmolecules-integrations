using NMolecules.DDD;

namespace NMolecules.DDD.Analyzers.Test.AggregateRootAnalyzerTests.SampleData
{
    [AggregateRoot]
    public class AggregateRootWithIdAsProperty
    {
        [Identity] 
        public string Id { get; } = "SomeId";
    }
}