using FluentValidation;
using ProductManagementSystem.Application.Requests;

namespace ProductManagementSystem.Application.Validators.Products;
public class LaptopRequestValidator : AbstractValidator<LaptopRequest>
{
    public LaptopRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Processor)
            .NotNull().WithMessage("Processor is required.");
        RuleFor(x => x.Processor.Brand)
            .IsInEnum().WithMessage("Processor brand is invalid.")
            .When(x => x.Processor != null);
        RuleFor(x => x.Processor.Model)
            .NotEmpty().WithMessage("Processor model is required.")
            .MaximumLength(100).WithMessage("Processor model must not exceed 100 characters.")
            .When(x => x.Processor != null);
        RuleFor(x => x.Processor.CoreCount)
            .GreaterThan(0).WithMessage("Processor core count must be greater than zero.")
            .When(x => x.Processor != null);
        RuleFor(x => x.Processor.ClockSpeedGHz)
            .GreaterThan(0).WithMessage("Processor clock speed must be greater than zero.")
            .When(x => x.Processor != null);

        RuleFor(x => x.RamSize)
            .IsInEnum().WithMessage("RAM size is invalid.");

        RuleFor(x => x.StorageCapacity)
            .NotNull().WithMessage("Storage capacity is required.");
        RuleFor(x => x.StorageCapacity.Unit)
            .IsInEnum().WithMessage("Storage unit is invalid.")
            .When(x => x.StorageCapacity != null);
        RuleFor(x => x.StorageCapacity.Value)
            .GreaterThan(0).WithMessage("Storage capacity must be greater than zero.")
            .When(x => x.StorageCapacity != null);

        RuleFor(x => x.OperatingSystem)
            .IsInEnum().WithMessage("Operating system is invalid.");

        RuleFor(x => x.GraphicsCard)
            .NotEmpty().WithMessage("Graphics card is required.")
            .MaximumLength(100).WithMessage("Graphics card must not exceed 100 characters.");

        RuleFor(x => x.ScreenSize)
            .GreaterThan(7.0).WithMessage("Screen size must be greater than 7 inches.")
            .LessThan(20.0).WithMessage("Screen size must be less than 20 inches.");

        RuleFor(x => x.BatteryLife)
            .GreaterThan(0).WithMessage("Battery life must be greater than zero.");

        RuleFor(x => x.WebcamQuality)
            .NotEmpty().WithMessage("Webcam quality is required.")
            .MaximumLength(50).WithMessage("Webcam quality must not exceed 50 characters.");
    }
}
