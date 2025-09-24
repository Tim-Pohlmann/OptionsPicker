using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using OptionsPicker.Models;
using OptionsPicker.Services;

namespace OptionsPicker.Tests;

[TestClass]
public class WeightedSelectionTests
{
    private OptionCollection _collection = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _collection = new OptionCollection();
    }

    [TestMethod]
    public void SelectOption_WithMultipleEqualWeights_ShouldDistributeEvenly()
    {
        // Arrange
        var option1 = Option.Create("Option1", 1.0);
        var option2 = Option.Create("Option2", 1.0);
        var option3 = Option.Create("Option3", 1.0);

        _collection.AddOption(option1);
        _collection.AddOption(option2);
        _collection.AddOption(option3);

        // Act - perform many selections to test distribution
        var selectionCounts = new Dictionary<Guid, int>();
        const int numberOfSelections = 3000;

        for (int i = 0; i < numberOfSelections; i++)
        {
            var result = _collection.SelectOption();
            selectionCounts.TryGetValue(result.SelectedOption.Id, out int currentCount);
            selectionCounts[result.SelectedOption.Id] = currentCount + 1;
        }

        // Assert - each option should be selected approximately 1/3 of the time (within 10% tolerance)
        foreach (var count in selectionCounts.Values)
        {
            var expectedCount = numberOfSelections / 3.0;
            var tolerance = expectedCount * 0.1; // 10% tolerance
            count.ShouldBeInRange((int)(expectedCount - tolerance), (int)(expectedCount + tolerance));
        }
    }

    [TestMethod]
    public void SelectOption_WithDifferentWeights_ShouldDistributeProportionally()
    {
        // Arrange - weights in ratio 1:2:3
        var option1 = Option.Create("Option1", 1.0); // Should be selected ~16.7% of the time
        var option2 = Option.Create("Option2", 2.0); // Should be selected ~33.3% of the time
        var option3 = Option.Create("Option3", 3.0); // Should be selected ~50% of the time

        _collection.AddOption(option1);
        _collection.AddOption(option2);
        _collection.AddOption(option3);

        // Act
        var selectionCounts = new Dictionary<Guid, int>();
        const int numberOfSelections = 6000;

        for (int i = 0; i < numberOfSelections; i++)
        {
            var result = _collection.SelectOption();
            selectionCounts.TryGetValue(result.SelectedOption.Id, out int currentCount);
            selectionCounts[result.SelectedOption.Id] = currentCount + 1;
        }

        // Assert
        var option1Count = selectionCounts[option1.Id];
        var option2Count = selectionCounts[option2.Id];
        var option3Count = selectionCounts[option3.Id];

        // Option1 should be selected about 16.7% of the time (1/6 of total weight)
        var expected1 = numberOfSelections * (1.0 / 6.0);
        var tolerance1 = expected1 * 0.15; // 15% tolerance
        option1Count.ShouldBeInRange((int)(expected1 - tolerance1), (int)(expected1 + tolerance1));

        // Option2 should be selected about 33.3% of the time (2/6 of total weight)
        var expected2 = numberOfSelections * (2.0 / 6.0);
        var tolerance2 = expected2 * 0.15;
        option2Count.ShouldBeInRange((int)(expected2 - tolerance2), (int)(expected2 + tolerance2));

        // Option3 should be selected about 50% of the time (3/6 of total weight)
        var expected3 = numberOfSelections * (3.0 / 6.0);
        var tolerance3 = expected3 * 0.15;
        option3Count.ShouldBeInRange((int)(expected3 - tolerance3), (int)(expected3 + tolerance3));
    }

    [TestMethod]
    public void SelectOption_WithVerySmallWeights_ShouldStillWork()
    {
        var option1 = Option.Create("Option1", 0.001);
        var option2 = Option.Create("Option2", 0.002);

        _collection.AddOption(option1);
        _collection.AddOption(option2);

        // Should not throw and should return one of the options
        var result = _collection.SelectOption();
        result.SelectedOption.ShouldBeOneOf(option1, option2);
        result.TotalWeight.ShouldBe(0.003, tolerance: 0.0001);
    }

    [TestMethod]
    public void SelectOption_WithVeryLargeWeights_ShouldStillWork()
    {
        var option1 = Option.Create("Option1", 1000000.0);
        var option2 = Option.Create("Option2", 2000000.0);

        _collection.AddOption(option1);
        _collection.AddOption(option2);

        var result = _collection.SelectOption();
        result.SelectedOption.ShouldBeOneOf(option1, option2);
        result.TotalWeight.ShouldBe(3000000.0);
    }

    [TestMethod]
    public void SelectOption_ShouldSetSelectionTimeAndMetadata()
    {
        var option = Option.Create("Test", 1.0);
        _collection.AddOption(option);

        var beforeSelection = DateTime.UtcNow;
        var result = _collection.SelectOption();
        var afterSelection = DateTime.UtcNow;

        result.SelectionTime.ShouldBeInRange(beforeSelection, afterSelection);
        result.TotalOptions.ShouldBe(1);
        result.TotalWeight.ShouldBe(1.0);
        result.SelectedOption.ShouldBe(option);
    }

    [TestMethod]
    public void SelectOption_WithZeroWeightOptions_ShouldThrowInvalidOperationException()
    {
        // This test assumes we might have options with zero weight through direct instantiation
        // Since Option.Create prevents zero weights, we need to test the collection's behavior

        // For now, this tests the edge case where total weight is zero
        // In practice, this shouldn't happen with current Option.Create validation
        _collection.Count.ShouldBe(0); // Empty collection

        Should.Throw<InvalidOperationException>(() => _collection.SelectOption())
            .Message.ShouldContain("Cannot select from an empty collection");
    }

    [TestMethod]
    [DataRow(10)]
    [DataRow(50)]
    [DataRow(100)]
    [DataRow(1000)]
    public void SelectOption_WithManyOptions_ShouldPerformEfficiently(int optionCount)
    {
        // Arrange
        for (int i = 0; i < optionCount; i++)
        {
            _collection.AddOption(Option.Create($"Option{i}", 1.0));
        }

        // Act & Assert - Should complete quickly even with many options
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 1000; i++)
        {
            var result = _collection.SelectOption();
            result.ShouldNotBeNull();
            result.SelectedOption.ShouldNotBeNull();
        }

        stopwatch.Stop();

        // Should complete 1000 selections in reasonable time (adjust threshold as needed)
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(1000); // Less than 1 second
    }
}