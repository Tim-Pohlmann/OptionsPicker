using OptionsPicker.Models;

namespace OptionsPicker.Services;

public class OptionCollection : IOptionCollection
{
    private readonly List<Option> _options = new();
    private readonly Random _random = new();

    public IReadOnlyList<Option> Options => _options.AsReadOnly();
    public int Count => _options.Count;

    public void AddOption(Option option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));

        if (ContainsOption(option.Name))
            throw new InvalidOperationException($"Option with name '{option.Name}' already exists");

        _options.Add(option);
    }

    public void RemoveOption(Guid optionId)
    {
        var optionToRemove = _options.FirstOrDefault(o => o.Id == optionId);
        if (optionToRemove != null)
        {
            _options.Remove(optionToRemove);
        }
    }

    public void UpdateOption(Option option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));

        var existingOption = GetOption(option.Id);
        if (existingOption == null)
            throw new InvalidOperationException($"Option with ID '{option.Id}' not found");

        // Check for name conflicts (excluding the current option)
        if (_options.Any(o => o.Id != option.Id && o.Name.Equals(option.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Option with name '{option.Name}' already exists");

        var index = _options.FindIndex(o => o.Id == option.Id);
        _options[index] = option;
    }

    public Option? GetOption(Guid optionId)
    {
        return _options.FirstOrDefault(o => o.Id == optionId);
    }

    public SelectionResult SelectOption()
    {
        if (!_options.Any())
            throw new InvalidOperationException("Cannot select from an empty collection");

        var totalWeight = _options.Sum(o => o.Weight);
        if (totalWeight <= 0)
            throw new InvalidOperationException("Total weight must be greater than 0");

        var randomValue = _random.NextDouble() * totalWeight;
        var cumulativeWeight = 0.0;

        foreach (var option in _options)
        {
            cumulativeWeight += option.Weight;
            if (randomValue <= cumulativeWeight)
            {
                return new SelectionResult
                {
                    SelectedOption = option,
                    SelectionTime = DateTime.UtcNow,
                    TotalOptions = _options.Count,
                    TotalWeight = totalWeight
                };
            }
        }

        // Fallback to last option (should rarely happen due to floating-point precision)
        return new SelectionResult
        {
            SelectedOption = _options.Last(),
            SelectionTime = DateTime.UtcNow,
            TotalOptions = _options.Count,
            TotalWeight = totalWeight
        };
    }

    public void ClearOptions()
    {
        _options.Clear();
    }

    public bool ContainsOption(string name)
    {
        return _options.Any(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}