using NMolecules.DDD;

namespace NMolecules.DDD.Analyzers.Test.AggregateRootAnalyzerTests.SampleData
{
    [AggregateRoot]
    public class AggregateRootWithIdAsField
    {
        [Identity] 
        public string Id = "SomeId";
    }
}