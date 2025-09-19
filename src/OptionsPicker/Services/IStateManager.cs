using OptionsPicker.Models;

namespace OptionsPicker.Services;

public interface IStateManager
{
    IReadOnlyList<Option> CurrentOptions { get; }
    event EventHandler<IReadOnlyList<Option>>? OptionsChanged;
    
    void UpdateOptions(IReadOnlyList<Option> options);
    void AddOption(Option option);
    void RemoveOption(Guid optionId);
    void UpdateOption(Option option);
    
    string SerializeToUrl();
    void LoadFromUrl(string urlParameters);
    void LoadFromUrl();
}