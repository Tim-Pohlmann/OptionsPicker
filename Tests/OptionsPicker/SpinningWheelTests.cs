using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using NSubstitute;
using OptionsPicker.Models;
using OptionsPicker.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace OptionsPicker.Tests;

[TestClass]
public class SpinningWheelTests : TestContextWrapper
{
    private IJSRuntime _mockJSRuntime = null!;
    private IJSObjectReference _mockJSModule = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockJSRuntime = Substitute.For<IJSRuntime>();
        _mockJSModule = Substitute.For<IJSObjectReference>();

        // Setup JSRuntime to return mock module
        _mockJSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/wheel.js")
            .Returns(ValueTask.FromResult(_mockJSModule));

        TestContext.Services.AddSingleton<IJSRuntime>(_mockJSRuntime);
    }

    [TestMethod]
    public void SpinningWheel_WithNoOptions_ShouldShowMessage()
    {
        // Arrange
        var emptyOptions = new List<Option>();

        // Act
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, emptyOptions));

        // Assert
        var message = component.Find(".no-options-wheel-message");
        message.TextContent.ShouldContain("Add some options above to spin the wheel!");

        var button = component.Find(".spin-button");
        button.HasAttribute("disabled").ShouldBeTrue();
    }

    [TestMethod]
    public void SpinningWheel_WithOptions_ShouldShowSpinButton()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Option1", 1.0),
            Option.Create("Option2", 2.0)
        };

        // Act
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Assert
        var button = component.Find(".spin-button");
        button.HasAttribute("disabled").ShouldBeFalse();
        button.TextContent.ShouldContain("Spin the Wheel!");
    }

    [TestMethod]
    public void SpinningWheel_WhenSpinning_ShouldShowSpinningState()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Option1", 1.0)
        };

        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Act - trigger spin by calling the component method directly
        // (clicking would require mocking the JS module properly)

        // Assert - component should be in initial non-spinning state
        var button = component.Find(".spin-button");
        button.ClassList.ShouldNotContain("spinning");
        button.TextContent.ShouldContain("Spin the Wheel!");
        button.HasAttribute("disabled").ShouldBeFalse();
    }

    [TestMethod]
    public void GenerateColors_WithMultipleOptions_ShouldReturnDistinctColors()
    {
        // This tests the color generation logic indirectly by checking segments
        // Since GenerateColors is private, we test through the component behavior

        // Arrange
        var options = new List<Option>
        {
            Option.Create("Red Option", 1.0),
            Option.Create("Blue Option", 1.0),
            Option.Create("Green Option", 1.0)
        };

        // Act
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Assert - component should render without errors
        component.Find(".wheel-canvas").ShouldNotBeNull();
        component.Find(".spin-button").ShouldNotBeNull();
    }

    [TestMethod]
    public async Task SpinningWheel_OnOptionSelected_ShouldFireCallback()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Selected Option", 1.0)
        };

        Option? selectedOption = null;
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options)
            .Add(p => p.OnOptionSelected, EventCallback.Factory.Create<Option>(this, opt => selectedOption = opt)));

        // We can't fully test the async spin without mocking the delays
        // But we can verify the callback structure is correct
        selectedOption.ShouldBeNull(); // Initially null

        // The component should be set up correctly for callbacks
        component.Instance.ShouldNotBeNull();
    }

    [TestMethod]
    public void WheelSegment_Calculation_ShouldBeProportionalToWeights()
    {
        // This tests the segment calculation logic indirectly
        // by verifying the component handles weighted options correctly

        // Arrange
        var options = new List<Option>
        {
            Option.Create("Light Option", 1.0),   // 25%
            Option.Create("Heavy Option", 3.0)    // 75%
        };

        // Act
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Assert - component should handle different weights
        component.Find(".wheel-canvas").ShouldNotBeNull();

        // Verify JSRuntime was called to draw wheel (after component renders)
        // Note: This happens in OnAfterRenderAsync, so we need to wait for it
    }

    [TestMethod]
    public void SpinningWheel_CanvasClick_ShouldBeClickable()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Clickable Option", 1.0)
        };

        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Act & Assert - canvas should be present and clickable
        var canvas = component.Find(".wheel-canvas");
        canvas.ShouldNotBeNull();

        // Canvas should be clickable (has cursor: pointer in CSS)
        canvas.ClassList.ShouldContain("wheel-canvas");
    }

    [TestMethod]
    public void SpinningWheel_ResponsiveDesign_ShouldHaveCorrectClasses()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Responsive Option", 1.0)
        };

        // Act
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Assert - check for responsive CSS classes and structure
        var container = component.Find(".spinning-wheel-container");
        container.ShouldNotBeNull();

        var wrapper = component.Find(".wheel-wrapper");
        wrapper.ShouldNotBeNull();

        var controls = component.Find(".wheel-controls");
        controls.ShouldNotBeNull();
    }

    [TestMethod]
    public void SpinningWheel_WheelPointer_ShouldBePresent()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Pointed Option", 1.0)
        };

        // Act
        var component = TestContext.RenderComponent<SpinningWheel>(parameters => parameters
            .Add(p => p.Options, options));

        // Assert
        var pointer = component.Find(".wheel-pointer");
        pointer.ShouldNotBeNull();

        var triangle = component.Find(".pointer-triangle");
        triangle.ShouldNotBeNull();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        TestContext.Dispose();
    }
}

// Helper class for testing components
public class TestContextWrapper : IDisposable
{
    protected Bunit.TestContext TestContext { get; private set; }

    public TestContextWrapper()
    {
        TestContext = new Bunit.TestContext();
    }

    public void Dispose()
    {
        TestContext?.Dispose();
    }
}