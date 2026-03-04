using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Infrastructure;

[ApplicationLayer]
[ApplicationService]
[Entity]
public sealed class CombinedRoleMismatch
{
    [Identity]
    public string Id { get; } = Guid.NewGuid().ToString("N");
}

[DomainLayer]
[ValueObject]
public class CombinedBrokenSnapshot
{
    [Identity]
    public string SnapshotId { get; set; } = "snapshot";
}
