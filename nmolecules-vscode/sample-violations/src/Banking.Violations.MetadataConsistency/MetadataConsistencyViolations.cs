using NMolecules.DDD;

[assembly: BoundedContext(Id = "MetadataContext", Name = "Metadata Alpha")]
[module: BoundedContext(Id = "MetadataContext", Name = "Metadata Beta")]
[assembly: Module(Id = "SharedModule", Name = "Orders", BoundedContextId = "SalesContext")]
[module: Module(Id = "SharedModule", Name = "OrdersApi", BoundedContextId = "BillingContext")]

namespace Banking.Violations.MetadataConsistency;

public static class MetadataConsistencyViolations
{
}
