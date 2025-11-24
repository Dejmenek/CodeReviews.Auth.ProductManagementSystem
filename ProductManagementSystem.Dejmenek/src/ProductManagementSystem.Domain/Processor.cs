namespace ProductManagementSystem.Domain;
public sealed class Processor
{
    public Brand Brand { get; }
    public string Model { get; }
    public int CoreCount { get; }
    public double ClockSpeedGHz { get; }
    public Processor(Brand brand, string model, int coreCount, double clockSpeedGHz)
    {
        if (!Enum.IsDefined(typeof(Brand), brand))
            throw new ArgumentException("Invalid brand.", nameof(brand));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be null or empty.", nameof(model));
        if (coreCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(coreCount), "Core count must be greater than zero.");
        if (clockSpeedGHz <= 0)
            throw new ArgumentOutOfRangeException(nameof(clockSpeedGHz), "Clock speed must be greater than zero.");
        Brand = brand;
        Model = model;
        CoreCount = coreCount;
        ClockSpeedGHz = clockSpeedGHz;
    }
    public override string ToString() => $"{Brand} {Model}, {CoreCount} cores @ {ClockSpeedGHz} GHz";
}

public enum Brand
{
    Intel,
    AMD,
    Apple
}
