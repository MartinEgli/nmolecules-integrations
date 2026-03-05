using NMolecules.DDD;

[assembly: BoundedContext(Id = "Billing", Name = "Billing")]
[assembly: Module(Id = "DomainModel", Name = "Domain Model", BoundedContextId = "Billing")]
[module: Module(Id = "DomainModel", Name = "Domain Model", BoundedContextId = "Billing")]
