using NMolecules.DDD;

namespace NMolecules.Analyzers.Test.AggregateRootAnalyzerTests.SampleData
{
    public partial class AggregateRootWithIdAsPropertyInPartial
    {
        // SomeDomainLogic
    }
    
    [AggregateRoot]
    public partial class AggregateRootWithIdAsPropertyInPartial
    {
        [Identity] 
        public string Id { get; } = "SomeId";
    }
}