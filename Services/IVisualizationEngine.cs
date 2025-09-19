using OptionsPicker.Models;

namespace OptionsPicker.Services;

public interface IVisualizationEngine
{
    Task<SelectionResult> TriggerSelectionAsync();
    void UpdateOptions(IReadOnlyList<Option> options);
    event EventHandler<SelectionResult>? SelectionCompleted;
    bool IsAnimating { get; }
}