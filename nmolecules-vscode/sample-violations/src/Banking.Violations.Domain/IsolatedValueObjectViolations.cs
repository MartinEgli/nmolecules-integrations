using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Domain;

[DomainLayer]
[ValueObject]
public sealed class IdentityValueObject : IEquatable<IdentityValueObject>
{
    [Identity]
    public string ExternalId { get; } = "value-object-id";

    public bool Equals(IdentityValueObject? other)
    {
        return other is not null && other.ExternalId == ExternalId;
    }

    public override bool Equals(object? obj)
    {
        return obj is IdentityValueObject other && Equals(other);
    }

    public override int GetHashCode()
    {
        return ExternalId.GetHashCode();
    }
}

[DomainLayer]
[ValueObject]
public sealed class MutableValueObject : IEquatable<MutableValueObject>
{
    public string Code { get; set; } = "MUT";

    public bool Equals(MutableValueObject? other)
    {
        return other is not null && other.Code == Code;
    }

    public override bool Equals(object? obj)
    {
        return obj is MutableValueObject other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}

[DomainLayer]
[ValueObject]
public sealed class MissingEquatableValueObject
{
    public MissingEquatableValueObject(string code)
    {
        Code = code;
    }

    public string Code { get; }
}

[DomainLayer]
[ValueObject]
public class NotSealedValueObject : IEquatable<NotSealedValueObject>
{
    public NotSealedValueObject(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public bool Equals(NotSealedValueObject? other)
    {
        return other is not null && other.Code == Code;
    }

    public override bool Equals(object? obj)
    {
        return obj is NotSealedValueObject other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
