using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using NSubstitute;
using OptionsPicker.Models;
using OptionsPicker.Services;
using Microsoft.AspNetCore.Components;

namespace OptionsPicker.Tests;

public class TestNavigationManager : NavigationManager
{
    public TestNavigationManager(string baseUri, string uri) : base()
    {
        Initialize(baseUri, uri);
    }

    public string LastNavigatedUri { get; private set; } = string.Empty;
    public bool LastNavigateReplace { get; private set; }

    protected override void NavigateToCore(string uri, bool replace)
    {
        LastNavigatedUri = uri;
        LastNavigateReplace = replace;
    }

    protected override void NavigateToCore(string uri, Microsoft.AspNetCore.Components.NavigationOptions options)
    {
        LastNavigatedUri = uri;
        LastNavigateReplace = options.ReplaceHistoryEntry;
    }
}

[TestClass]
public class StateManagerTests
{
    private IOptionCollection _mockOptionCollection = null!;
    private TestNavigationManager _mockNavigationManager = null!;
    private StateManager _stateManager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockOptionCollection = Substitute.For<IOptionCollection>();
        _mockNavigationManager = new TestNavigationManager("https://localhost/", "https://localhost/");

        _stateManager = new StateManager(_mockOptionCollection, _mockNavigationManager);
    }

    [TestMethod]
    public void CurrentOptions_ShouldReturnOptionsFromCollection()
    {
        var expectedOptions = new List<Option>
        {
            Option.Create("Option1", 1.0),
            Option.Create("Option2", 2.0)
        };
        _mockOptionCollection.Options.Returns(expectedOptions);

        var result = _stateManager.CurrentOptions;

        result.ShouldBe(expectedOptions);
    }

    [TestMethod]
    public void AddOption_ShouldCallOptionCollectionAndTriggerEvent()
    {
        var option = Option.Create("Test Option", 1.0);
        var eventFired = false;

        _stateManager.OptionsChanged += (sender, options) => eventFired = true;

        // Setup mock to return the option when queried for serialization
        _mockOptionCollection.Options.Returns(new List<Option> { option });

        _stateManager.AddOption(option);

        _mockOptionCollection.Received(1).AddOption(option);
        eventFired.ShouldBeTrue();
        // Should navigate because the URL will change from base to base + query params
        _mockNavigationManager.LastNavigatedUri.ShouldContain("options=");
        _mockNavigationManager.LastNavigateReplace.ShouldBeTrue();
    }

    [TestMethod]
    public void RemoveOption_ShouldCallOptionCollectionAndTriggerEvent()
    {
        var optionId = Guid.NewGuid();
        var eventFired = false;

        // Start with an existing URL that has options, so removal will cause a change
        _mockNavigationManager = new TestNavigationManager("https://localhost/", "https://localhost/?options=existing");
        _stateManager = new StateManager(_mockOptionCollection, _mockNavigationManager);

        _stateManager.OptionsChanged += (sender, options) => eventFired = true;

        // Mock returns empty after removal
        _mockOptionCollection.Options.Returns(new List<Option>());

        _stateManager.RemoveOption(optionId);

        _mockOptionCollection.Received(1).RemoveOption(optionId);
        eventFired.ShouldBeTrue();
        // Should navigate to base URL (no options)
        _mockNavigationManager.LastNavigatedUri.ShouldBe("https://localhost/");
        _mockNavigationManager.LastNavigateReplace.ShouldBeTrue();
    }

    [TestMethod]
    public void UpdateOption_ShouldCallOptionCollectionAndTriggerEvent()
    {
        var option = Option.Create("Updated Option", 2.0);
        var eventFired = false;

        _stateManager.OptionsChanged += (sender, options) => eventFired = true;

        // Setup mock to return the updated option
        _mockOptionCollection.Options.Returns(new List<Option> { option });

        _stateManager.UpdateOption(option);

        _mockOptionCollection.Received(1).UpdateOption(option);
        eventFired.ShouldBeTrue();
        _mockNavigationManager.LastNavigatedUri.ShouldContain("options=");
        _mockNavigationManager.LastNavigateReplace.ShouldBeTrue();
    }

    [TestMethod]
    public void UpdateOptions_ShouldClearAndAddAllOptions()
    {
        var options = new List<Option>
        {
            Option.Create("Option1", 1.0),
            Option.Create("Option2", 2.0)
        };

        // Setup mock to return the options after they're added
        _mockOptionCollection.Options.Returns(options);

        _stateManager.UpdateOptions(options);

        _mockOptionCollection.Received(1).ClearOptions();
        _mockOptionCollection.Received(1).AddOption(options[0]);
        _mockOptionCollection.Received(1).AddOption(options[1]);
        _mockNavigationManager.LastNavigatedUri.ShouldContain("options=");
        _mockNavigationManager.LastNavigateReplace.ShouldBeTrue();
    }

    [TestMethod]
    public void SerializeToUrl_WithNoOptions_ShouldReturnEmptyString()
    {
        _mockOptionCollection.Options.Returns(new List<Option>());

        var result = _stateManager.SerializeToUrl();

        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void SerializeToUrl_WithOptions_ShouldReturnEncodedJson()
    {
        var options = new List<Option>
        {
            Option.Create("Option1", 1.0),
            Option.Create("Option2", 2.5)
        };
        _mockOptionCollection.Options.Returns(options);

        var result = _stateManager.SerializeToUrl();

        result.ShouldNotBeEmpty();
        result.ShouldContain("Option1");
        result.ShouldContain("Option2");
        // Should be URL encoded
        result.ShouldNotContain(" ");
    }

    [TestMethod]
    public void LoadFromUrl_WithEmptyString_ShouldLoadDefaultOptions()
    {
        _stateManager.LoadFromUrl("");

        // Should clear and add default options
        _mockOptionCollection.Received(1).ClearOptions();
        _mockOptionCollection.Received(3).AddOption(Arg.Any<Option>());
    }

    [TestMethod]
    public void LoadFromUrl_WithValidJson_ShouldLoadOptions()
    {
        var urlParams = "options=%5B%7B%22n%22%3A%22Test%22%2C%22w%22%3A1%7D%5D"; // [{"n":"Test","w":1}] encoded

        _stateManager.LoadFromUrl(urlParams);

        _mockOptionCollection.Received(1).ClearOptions();
        _mockOptionCollection.Received().AddOption(Arg.Is<Option>(o => o.Name == "Test" && o.Weight == 1.0));
    }

    [TestMethod]
    public void LoadFromUrl_WithMalformedJson_ShouldLoadDefaultOptions()
    {
        var urlParams = "options=invalid-json";

        _stateManager.LoadFromUrl(urlParams);

        // Should fall back to default options
        _mockOptionCollection.Received(1).ClearOptions();
        _mockOptionCollection.Received(3).AddOption(Arg.Any<Option>());
    }

    [TestMethod]
    public void RoundTripSerialization_ShouldPreserveOptions()
    {
        // Use real OptionCollection for this test
        var realCollection = new OptionCollection();
        var testNavManager = new TestNavigationManager("https://localhost/", "https://localhost/");
        var realStateManager = new StateManager(realCollection, testNavManager);

        var originalOptions = new List<Option>
        {
            Option.Create("Option 1", 1.0),
            Option.Create("Option 2 & Special", 2.5),
            Option.Create("Option/3", 0.5)
        };

        realStateManager.UpdateOptions(originalOptions);
        var serialized = realStateManager.SerializeToUrl();

        // Clear and reload
        realCollection.ClearOptions();
        realStateManager.LoadFromUrl($"options={serialized}");

        var loadedOptions = realStateManager.CurrentOptions;
        loadedOptions.Count.ShouldBe(3);

        loadedOptions.ShouldContain(o => o.Name == "Option 1" && o.Weight == 1.0);
        loadedOptions.ShouldContain(o => o.Name == "Option 2 & Special" && o.Weight == 2.5);
        loadedOptions.ShouldContain(o => o.Name == "Option/3" && o.Weight == 0.5);
    }

    [TestMethod]
    public void LoadFromUrl_WithInvalidWeights_ShouldSkipInvalidOptions()
    {
        // JSON with mix of valid and invalid options
        var urlParams = "options=%5B%7B%22n%22%3A%22Valid%22%2C%22w%22%3A1%7D%2C%7B%22n%22%3A%22Invalid%22%2C%22w%22%3A0%7D%5D";

        _stateManager.LoadFromUrl(urlParams);

        _mockOptionCollection.Received(1).ClearOptions();
        // Should only add the valid option
        _mockOptionCollection.Received(1).AddOption(Arg.Is<Option>(o => o.Name == "Valid"));
    }

    [TestMethod]
    public void LoadFromUrl_WithEmptyOptionsArray_ShouldLoadDefaultOptions()
    {
        var urlParams = "options=%5B%5D"; // [] encoded

        _stateManager.LoadFromUrl(urlParams);

        // Should fall back to default options
        _mockOptionCollection.Received(1).ClearOptions();
        _mockOptionCollection.Received(3).AddOption(Arg.Any<Option>());
    }
}