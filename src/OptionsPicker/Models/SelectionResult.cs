namespace OptionsPicker.Models;

public record SelectionResult
{
    public Option SelectedOption { get; init; } = null!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public double RandomValue { get; init; }
    
    public static SelectionResult Create(Option selectedOption, double randomValue)
    {
        return new SelectionResult
        {
            SelectedOption = selectedOption,
            RandomValue = randomValue
        };
    }
}