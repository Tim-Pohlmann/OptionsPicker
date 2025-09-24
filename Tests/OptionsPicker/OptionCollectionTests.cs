using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using OptionsPicker.Models;
using OptionsPicker.Services;

namespace OptionsPicker.Tests;

[TestClass]
public class OptionCollectionTests
{
    private OptionCollection _collection = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _collection = new OptionCollection();
    }

    [TestMethod]
    public void AddOption_WithValidOption_ShouldAddToCollection()
    {
        var option = Option.Create("Test Option", 1.0);

        _collection.AddOption(option);

        _collection.Count.ShouldBe(1);
        _collection.Options.ShouldContain(option);
    }

    [TestMethod]
    public void AddOption_WithNullOption_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => _collection.AddOption(null!));
    }

    [TestMethod]
    public void AddOption_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        var option1 = Option.Create("Test", 1.0);
        var option2 = Option.Create("Test", 2.0);

        _collection.AddOption(option1);

        Should.Throw<InvalidOperationException>(() => _collection.AddOption(option2))
            .Message.ShouldContain("Option with name 'Test' already exists");
    }

    [TestMethod]
    public void RemoveOption_WithExistingOption_ShouldRemoveFromCollection()
    {
        var option = Option.Create("Test Option", 1.0);
        _collection.AddOption(option);

        _collection.RemoveOption(option.Id);

        _collection.Count.ShouldBe(0);
        _collection.Options.ShouldNotContain(option);
    }

    [TestMethod]
    public void RemoveOption_WithNonExistentId_ShouldNotThrow()
    {
        var randomId = Guid.NewGuid();

        Should.NotThrow(() => _collection.RemoveOption(randomId));
        _collection.Count.ShouldBe(0);
    }

    [TestMethod]
    public void UpdateOption_WithValidOption_ShouldUpdateInCollection()
    {
        var originalOption = Option.Create("Original", 1.0);
        _collection.AddOption(originalOption);

        var updatedOption = originalOption with { Name = "Updated", Weight = 2.0 };
        _collection.UpdateOption(updatedOption);

        _collection.Count.ShouldBe(1);
        var retrievedOption = _collection.GetOption(originalOption.Id);
        retrievedOption!.Name.ShouldBe("Updated");
        retrievedOption.Weight.ShouldBe(2.0);
    }

    [TestMethod]
    public void UpdateOption_WithNullOption_ShouldThrowArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => _collection.UpdateOption(null!));
    }

    [TestMethod]
    public void UpdateOption_WithNonExistentOption_ShouldThrowInvalidOperationException()
    {
        var option = Option.Create("Test", 1.0);

        Should.Throw<InvalidOperationException>(() => _collection.UpdateOption(option));
    }

    [TestMethod]
    public void UpdateOption_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        var option1 = Option.Create("Option1", 1.0);
        var option2 = Option.Create("Option2", 2.0);
        _collection.AddOption(option1);
        _collection.AddOption(option2);

        var updatedOption = option2 with { Name = "Option1" };

        Should.Throw<InvalidOperationException>(() => _collection.UpdateOption(updatedOption))
            .Message.ShouldContain("Option with name 'Option1' already exists");
    }

    [TestMethod]
    public void GetOption_WithExistingId_ShouldReturnOption()
    {
        var option = Option.Create("Test", 1.0);
        _collection.AddOption(option);

        var retrievedOption = _collection.GetOption(option.Id);

        retrievedOption.ShouldBe(option);
    }

    [TestMethod]
    public void GetOption_WithNonExistentId_ShouldReturnNull()
    {
        var randomId = Guid.NewGuid();

        var retrievedOption = _collection.GetOption(randomId);

        retrievedOption.ShouldBeNull();
    }

    [TestMethod]
    public void ClearOptions_ShouldRemoveAllOptions()
    {
        _collection.AddOption(Option.Create("Option1", 1.0));
        _collection.AddOption(Option.Create("Option2", 2.0));

        _collection.ClearOptions();

        _collection.Count.ShouldBe(0);
        _collection.Options.ShouldBeEmpty();
    }

    [TestMethod]
    public void ContainsOption_WithExistingName_ShouldReturnTrue()
    {
        var option = Option.Create("Test Option", 1.0);
        _collection.AddOption(option);

        _collection.ContainsOption("Test Option").ShouldBeTrue();
    }

    [TestMethod]
    public void ContainsOption_WithNonExistentName_ShouldReturnFalse()
    {
        _collection.ContainsOption("Non-existent").ShouldBeFalse();
    }

    [TestMethod]
    public void ContainsOption_IsCaseInsensitive()
    {
        var option = Option.Create("Test Option", 1.0);
        _collection.AddOption(option);

        _collection.ContainsOption("test option").ShouldBeTrue();
        _collection.ContainsOption("TEST OPTION").ShouldBeTrue();
    }

    [TestMethod]
    public void SelectOption_WithEmptyCollection_ShouldThrowInvalidOperationException()
    {
        Should.Throw<InvalidOperationException>(() => _collection.SelectOption())
            .Message.ShouldContain("Cannot select from an empty collection");
    }

    [TestMethod]
    public void SelectOption_WithSingleOption_ShouldReturnThatOption()
    {
        var option = Option.Create("Only Option", 1.0);
        _collection.AddOption(option);

        var result = _collection.SelectOption();

        result.SelectedOption.ShouldBe(option);
        result.TotalOptions.ShouldBe(1);
        result.TotalWeight.ShouldBe(1.0);
    }
}