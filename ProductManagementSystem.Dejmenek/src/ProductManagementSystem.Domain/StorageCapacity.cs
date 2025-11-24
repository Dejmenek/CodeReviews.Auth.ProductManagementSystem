namespace ProductManagementSystem.Domain;
public sealed class StorageCapacity : IEquatable<StorageCapacity>
{
    public int Value { get; }
    public StorageUnit Unit { get; }

    public StorageCapacity(int value, StorageUnit unit)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Storage capacity must be greater than zero.");
        if (!Enum.IsDefined(typeof(StorageUnit), unit))
            throw new ArgumentException("Invalid storage unit.", nameof(unit));
        Value = value;
        Unit = unit;
    }

    public int ToGigabytes()
    {
        return Unit switch
        {
            StorageUnit.GB => Value,
            StorageUnit.TB => Value * 1024,
            _ => throw new NotSupportedException($"Unsupported storage unit: {Unit}")
        };
    }

    public override string ToString() => $"{Value} {Unit}";

    public bool Equals(StorageCapacity? other)
    {
        return other is not null && ToGigabytes() == other.ToGigabytes();
    }

    public override bool Equals(object? obj) => Equals(obj as StorageCapacity);
    public override int GetHashCode() => ToGigabytes().GetHashCode();
}

public enum StorageUnit
{
    GB,
    TB
}
