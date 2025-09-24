using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Components;
using OptionsPicker.Models;

namespace OptionsPicker.Services;

public class StateManager : IStateManager
{
    private readonly IOptionCollection _optionCollection;
    private readonly NavigationManager _navigationManager;
    private const string OptionsParameterName = "options";

    public StateManager(IOptionCollection optionCollection, NavigationManager navigationManager)
    {
        _optionCollection = optionCollection;
        _navigationManager = navigationManager;
    }

    public IReadOnlyList<Option> CurrentOptions => _optionCollection.Options;

    public event EventHandler<IReadOnlyList<Option>>? OptionsChanged;

    public void UpdateOptions(IReadOnlyList<Option> options)
    {
        _optionCollection.ClearOptions();
        foreach (var option in options)
        {
            _optionCollection.AddOption(option);
        }
        OnOptionsChanged();
        UpdateUrl();
    }

    public void AddOption(Option option)
    {
        _optionCollection.AddOption(option);
        OnOptionsChanged();
        UpdateUrl();
    }

    public void RemoveOption(Guid optionId)
    {
        _optionCollection.RemoveOption(optionId);
        OnOptionsChanged();
        UpdateUrl();
    }

    public void UpdateOption(Option option)
    {
        _optionCollection.UpdateOption(option);
        OnOptionsChanged();
        UpdateUrl();
    }

    public string SerializeToUrl()
    {
        if (!_optionCollection.Options.Any())
            return string.Empty;

        var optionData = _optionCollection.Options.Select(o => new
        {
            n = o.Name,
            w = o.Weight
        }).ToArray();

        var json = JsonSerializer.Serialize(optionData);
        return HttpUtility.UrlEncode(json);
    }

    public void LoadFromUrl(string urlParameters)
    {
        if (string.IsNullOrWhiteSpace(urlParameters))
        {
            LoadDefaultOptions();
            return;
        }

        try
        {
            var uri = new Uri($"http://localhost?{urlParameters}");
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            var optionsJson = queryParams[OptionsParameterName];

            if (string.IsNullOrEmpty(optionsJson))
            {
                LoadDefaultOptions();
                return;
            }

            var decodedJson = HttpUtility.UrlDecode(optionsJson);
            var optionData = JsonSerializer.Deserialize<dynamic[]>(decodedJson);

            if (optionData == null)
            {
                LoadDefaultOptions();
                return;
            }

            var options = new List<Option>();
            foreach (var item in optionData)
            {
                var element = (JsonElement)item;
                if (element.TryGetProperty("n", out var nameProperty) &&
                    element.TryGetProperty("w", out var weightProperty) &&
                    nameProperty.ValueKind == JsonValueKind.String &&
                    weightProperty.ValueKind == JsonValueKind.Number)
                {
                    var name = nameProperty.GetString();
                    var weight = weightProperty.GetDouble();

                    if (!string.IsNullOrWhiteSpace(name) && weight > 0)
                    {
                        options.Add(Option.Create(name, weight));
                    }
                }
            }

            if (options.Any())
            {
                UpdateOptions(options);
            }
            else
            {
                LoadDefaultOptions();
            }
        }
        catch
        {
            LoadDefaultOptions();
        }
    }

    public void LoadFromUrl()
    {
        var uri = new Uri(_navigationManager.Uri);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);
        var optionsJson = queryParams[OptionsParameterName];

        LoadFromUrl($"{OptionsParameterName}={optionsJson}");
    }

    private void UpdateUrl()
    {
        var serializedOptions = SerializeToUrl();
        var baseUri = new Uri(_navigationManager.BaseUri);
        var currentUri = new Uri(_navigationManager.Uri);

        var newUrl = string.IsNullOrEmpty(serializedOptions)
            ? baseUri.ToString()
            : $"{baseUri}?{OptionsParameterName}={serializedOptions}";

        if (currentUri.ToString() != newUrl)
        {
            _navigationManager.NavigateTo(newUrl, replace: true);
        }
    }

    private void LoadDefaultOptions()
    {
        var defaultOptions = new[]
        {
            Option.Create("Option 1", 1.0),
            Option.Create("Option 2", 1.0),
            Option.Create("Option 3", 1.0)
        };

        // Update options without triggering URL update to avoid recursion
        _optionCollection.ClearOptions();
        foreach (var option in defaultOptions)
        {
            _optionCollection.AddOption(option);
        }
        OnOptionsChanged();
    }

    private void OnOptionsChanged()
    {
        OptionsChanged?.Invoke(this, CurrentOptions);
    }
}