using System.ComponentModel.DataAnnotations;

namespace OptionsPicker.Models;

public record Option
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;
    
    [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    public double Weight { get; init; } = 1.0;
    
    public static Option Create(string name, double weight = 1.0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Option name cannot be empty", nameof(name));
        
        if (weight <= 0)
            throw new ArgumentException("Weight must be greater than 0", nameof(weight));
        
        return new Option { Name = name.Trim(), Weight = weight };
    }
}