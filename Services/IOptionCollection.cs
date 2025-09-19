using OptionsPicker.Models;

namespace OptionsPicker.Services;

public interface IOptionCollection
{
    IReadOnlyList<Option> Options { get; }
    int Count { get; }
    
    void AddOption(Option option);
    void RemoveOption(Guid optionId);
    void UpdateOption(Option option);
    Option? GetOption(Guid optionId);
    
    SelectionResult SelectOption();
    void ClearOptions();
    bool ContainsOption(string name);
}