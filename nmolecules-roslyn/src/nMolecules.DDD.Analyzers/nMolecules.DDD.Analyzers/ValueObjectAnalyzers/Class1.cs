namespace NMolecules.DDD.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [Factory]
    public class SomeFactory
{
}

[ValueObject]
public sealed class InvalidValueObject : IEquatable<InvalidValueObject>
{
    private readonly SomeFactory factory;
    public InvalidValueObject(SomeFactory value)
    {
        Value = value;
    }

    public SomeFactory Value { get; }

    public SomeFactory SomeMethod(SomeFactory factory)
    {
        var someFactory = new SomeFactory();
        return someFactory;
    }

    public bool Equals(InvalidValueObject other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is InvalidValueObject other && Equals(other);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
}