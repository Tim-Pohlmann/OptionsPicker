using OptionsPicker.Models;

namespace OptionsPicker.Services;

public class SelectionService : ISelectionService
{
    private readonly IOptionCollection _optionCollection;
    private readonly List<SelectionResult> _selectionHistory = new();
    private readonly Dictionary<string, int> _selectionCounts = new();
    private bool _isSelecting = false;

    public event EventHandler<SelectionResult>? SelectionMade;
    public event EventHandler<bool>? SelectionStateChanged;

    public IReadOnlyList<SelectionResult> SelectionHistory => _selectionHistory.AsReadOnly();
    public bool IsSelecting => _isSelecting;
    public SelectionResult? LastSelection => _selectionHistory.LastOrDefault();

    public SelectionService(IOptionCollection optionCollection)
    {
        _optionCollection = optionCollection;
    }

    public async Task<SelectionResult> SelectRandomOptionAsync(CancellationToken cancellationToken = default)
    {
        if (_isSelecting)
            throw new InvalidOperationException("Selection is already in progress");

        SetSelectingState(true);

        try
        {
            // Add a small delay for UI feedback (animation opportunity)
            await Task.Delay(100, cancellationToken);

            // Perform the actual selection
            var result = _optionCollection.SelectOption();

            // Add to history (keep last 20)
            _selectionHistory.Add(result);
            if (_selectionHistory.Count > 20)
            {
                _selectionHistory.RemoveAt(0);
            }

            // Update selection counts for statistics
            var optionName = result.SelectedOption.Name;
            _selectionCounts.TryGetValue(optionName, out int currentCount);
            _selectionCounts[optionName] = currentCount + 1;

            // Add a longer delay to allow for animations
            await Task.Delay(1000, cancellationToken);

            SelectionMade?.Invoke(this, result);
            return result;
        }
        finally
        {
            SetSelectingState(false);
        }
    }

    public void ClearHistory()
    {
        _selectionHistory.Clear();
    }

    public Dictionary<string, int> GetSelectionCounts()
    {
        return new Dictionary<string, int>(_selectionCounts);
    }

    public Dictionary<string, double> GetSelectionFairness()
    {
        var fairness = new Dictionary<string, double>();

        if (!_selectionHistory.Any())
            return fairness;

        var totalSelections = _selectionHistory.Count;
        var currentOptions = _optionCollection.Options.ToList();
        var totalWeight = currentOptions.Sum(o => o.Weight);

        foreach (var option in currentOptions)
        {
            var expectedPercentage = (option.Weight / totalWeight) * 100;
            var actualCount = _selectionCounts.GetValueOrDefault(option.Name, 0);
            var actualPercentage = (actualCount / (double)totalSelections) * 100;

            // Fairness score: closer to 0 means more fair (expected vs actual)
            var fairnessScore = Math.Abs(expectedPercentage - actualPercentage);
            fairness[option.Name] = fairnessScore;
        }

        return fairness;
    }

    public void ResetStatistics()
    {
        _selectionHistory.Clear();
        _selectionCounts.Clear();
    }

    private void SetSelectingState(bool isSelecting)
    {
        if (_isSelecting != isSelecting)
        {
            _isSelecting = isSelecting;
            SelectionStateChanged?.Invoke(this, isSelecting);
        }
    }
}