namespace OptionsPicker.Models;

public record SelectionResult
{
    public Option SelectedOption { get; init; } = null!;
    public DateTime SelectionTime { get; init; } = DateTime.UtcNow;
    public double RandomValue { get; init; }
    public int TotalOptions { get; init; }
    public double TotalWeight { get; init; }

    public static SelectionResult Create(Option selectedOption, double randomValue, int totalOptions = 0, double totalWeight = 0.0)
    {
        return new SelectionResult
        {
            SelectedOption = selectedOption,
            RandomValue = randomValue,
            TotalOptions = totalOptions,
            TotalWeight = totalWeight
        };
    }
}