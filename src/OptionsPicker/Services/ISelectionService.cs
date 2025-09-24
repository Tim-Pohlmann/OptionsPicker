using OptionsPicker.Models;

namespace OptionsPicker.Services;

public interface ISelectionService
{
    event EventHandler<SelectionResult>? SelectionMade;
    event EventHandler<bool>? SelectionStateChanged;

    IReadOnlyList<SelectionResult> SelectionHistory { get; }
    bool IsSelecting { get; }
    SelectionResult? LastSelection { get; }

    Task<SelectionResult> SelectRandomOptionAsync(CancellationToken cancellationToken = default);
    void ClearHistory();
    Dictionary<string, int> GetSelectionCounts();
    Dictionary<string, double> GetSelectionFairness();
    void ResetStatistics();
}