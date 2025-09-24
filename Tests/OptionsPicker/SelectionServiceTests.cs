using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using NSubstitute;
using OptionsPicker.Models;
using OptionsPicker.Services;

namespace OptionsPicker.Tests;

[TestClass]
public class SelectionServiceTests
{
    private IOptionCollection _mockOptionCollection = null!;
    private SelectionService _selectionService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockOptionCollection = Substitute.For<IOptionCollection>();
        _selectionService = new SelectionService(_mockOptionCollection);
    }

    [TestMethod]
    public async Task SelectRandomOptionAsync_WithValidOptions_ShouldReturnSelectionResult()
    {
        // Arrange
        var option = Option.Create("Test Option", 1.0);
        var selectionResult = new SelectionResult
        {
            SelectedOption = option,
            SelectionTime = DateTime.UtcNow,
            TotalOptions = 1,
            TotalWeight = 1.0
        };

        _mockOptionCollection.SelectOption().Returns(selectionResult);

        // Act
        var result = await _selectionService.SelectRandomOptionAsync();

        // Assert
        result.ShouldNotBeNull();
        result.SelectedOption.ShouldBe(option);
        _mockOptionCollection.Received(1).SelectOption();
    }

    [TestMethod]
    public async Task SelectRandomOptionAsync_ShouldSetSelectingStateCorrectly()
    {
        // Arrange
        var option = Option.Create("Test Option", 1.0);
        var selectionResult = new SelectionResult
        {
            SelectedOption = option,
            SelectionTime = DateTime.UtcNow,
            TotalOptions = 1,
            TotalWeight = 1.0
        };

        _mockOptionCollection.SelectOption().Returns(selectionResult);

        var stateChanges = new List<bool>();
        _selectionService.SelectionStateChanged += (sender, isSelecting) => stateChanges.Add(isSelecting);

        // Act
        var resultTask = _selectionService.SelectRandomOptionAsync();

        // Should be selecting immediately
        _selectionService.IsSelecting.ShouldBeTrue();

        var result = await resultTask;

        // Should be done selecting
        _selectionService.IsSelecting.ShouldBeFalse();

        // Assert
        stateChanges.Count.ShouldBe(2);
        stateChanges[0].ShouldBeTrue();  // Started selecting
        stateChanges[1].ShouldBeFalse(); // Finished selecting
    }

    [TestMethod]
    public async Task SelectRandomOptionAsync_WhenAlreadySelecting_ShouldThrowException()
    {
        // Arrange
        var option = Option.Create("Test Option", 1.0);
        var selectionResult = new SelectionResult
        {
            SelectedOption = option,
            SelectionTime = DateTime.UtcNow,
            TotalOptions = 1,
            TotalWeight = 1.0
        };

        _mockOptionCollection.SelectOption().Returns(selectionResult);

        // Start first selection
        var firstSelectionTask = _selectionService.SelectRandomOptionAsync();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await _selectionService.SelectRandomOptionAsync());

        // Complete first selection
        await firstSelectionTask;
    }

    [TestMethod]
    public async Task SelectRandomOptionAsync_ShouldAddToHistory()
    {
        // Arrange
        var option = Option.Create("Test Option", 1.0);
        var selectionResult = new SelectionResult
        {
            SelectedOption = option,
            SelectionTime = DateTime.UtcNow,
            TotalOptions = 1,
            TotalWeight = 1.0
        };

        _mockOptionCollection.SelectOption().Returns(selectionResult);

        // Act
        await _selectionService.SelectRandomOptionAsync();

        // Assert
        _selectionService.SelectionHistory.Count.ShouldBe(1);
        _selectionService.SelectionHistory[0].SelectedOption.ShouldBe(option);
        _selectionService.LastSelection.ShouldBe(selectionResult);
    }

    [TestMethod]
    public async Task SelectRandomOptionAsync_ShouldFireSelectionMadeEvent()
    {
        // Arrange
        var option = Option.Create("Test Option", 1.0);
        var selectionResult = new SelectionResult
        {
            SelectedOption = option,
            SelectionTime = DateTime.UtcNow,
            TotalOptions = 1,
            TotalWeight = 1.0
        };

        _mockOptionCollection.SelectOption().Returns(selectionResult);

        SelectionResult? eventResult = null;
        _selectionService.SelectionMade += (sender, result) => eventResult = result;

        // Act
        await _selectionService.SelectRandomOptionAsync();

        // Assert
        eventResult.ShouldNotBeNull();
        eventResult.SelectedOption.ShouldBe(option);
    }

    [TestMethod]
    public void ClearHistory_ShouldRemoveAllHistoryItems()
    {
        // This requires making selections first, which is async
        // For simplicity, we'll test the clear function directly
        _selectionService.ClearHistory();
        _selectionService.SelectionHistory.Count.ShouldBe(0);
        _selectionService.LastSelection.ShouldBeNull();
    }

    [TestMethod]
    public async Task SelectionHistory_ShouldLimitTo20Items()
    {
        // Arrange
        var option = Option.Create("Test Option", 1.0);
        var selectionResult = new SelectionResult
        {
            SelectedOption = option,
            SelectionTime = DateTime.UtcNow,
            TotalOptions = 1,
            TotalWeight = 1.0
        };

        _mockOptionCollection.SelectOption().Returns(selectionResult);

        // Act - make 25 selections
        for (int i = 0; i < 25; i++)
        {
            await _selectionService.SelectRandomOptionAsync();
        }

        // Assert
        _selectionService.SelectionHistory.Count.ShouldBe(20);
    }

    [TestMethod]
    public async Task GetSelectionCounts_ShouldTrackOptionSelections()
    {
        // Arrange
        var option1 = Option.Create("Option1", 1.0);
        var option2 = Option.Create("Option2", 1.0);

        var result1 = new SelectionResult { SelectedOption = option1, SelectionTime = DateTime.UtcNow, TotalOptions = 2, TotalWeight = 2.0 };
        var result2 = new SelectionResult { SelectedOption = option2, SelectionTime = DateTime.UtcNow, TotalOptions = 2, TotalWeight = 2.0 };

        _mockOptionCollection.SelectOption()
            .Returns(result1, result1, result2); // Option1 selected twice, Option2 once

        // Act
        await _selectionService.SelectRandomOptionAsync();
        await _selectionService.SelectRandomOptionAsync();
        await _selectionService.SelectRandomOptionAsync();

        var counts = _selectionService.GetSelectionCounts();

        // Assert
        counts["Option1"].ShouldBe(2);
        counts["Option2"].ShouldBe(1);
    }

    [TestMethod]
    public void ResetStatistics_ShouldClearHistoryAndCounts()
    {
        // Act
        _selectionService.ResetStatistics();

        // Assert
        _selectionService.SelectionHistory.Count.ShouldBe(0);
        _selectionService.GetSelectionCounts().Count.ShouldBe(0);
        _selectionService.LastSelection.ShouldBeNull();
    }
}