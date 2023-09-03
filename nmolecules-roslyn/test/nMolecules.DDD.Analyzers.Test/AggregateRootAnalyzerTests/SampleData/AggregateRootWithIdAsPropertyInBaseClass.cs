using NMolecules.DDD;

namespace NMolecules.DDD.Analyzers.Test.AggregateRootAnalyzerTests.SampleData
{
    [AggregateRoot]
    public class AggregateRootWithIdAsProperty : AggregateRootWithIdAsPropertyBase
    {
        [Identity] 
        public string Id { get; } = "SomeId";
    }
    
    public class AggregateRootWithIdAsPropertyBase
    {
        [Identity] 
        public string Id { get; } = "SomeId";
    }
}