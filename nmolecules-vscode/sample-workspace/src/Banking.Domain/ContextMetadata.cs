using NMolecules.DDD;

[assembly: BoundedContext(Id = "Billing", Name = "Billing")]
[module: Module(Id = "DomainModel", Name = "Domain Model", BoundedContextId = "Billing")]
