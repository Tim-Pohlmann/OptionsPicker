using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using OptionsPicker.Models;
using OptionsPicker.Services;

namespace OptionsPicker.Tests;

[TestClass]
public class FileManagerTests
{
    private FileManager _fileManager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _fileManager = new FileManager();
    }

    [TestMethod]
    public void ExportOptions_WithValidOptions_ShouldReturnFormattedText()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Pizza", 3.0),
            Option.Create("Burger", 2.0),
            Option.Create("Salad", 1.0)
        };

        // Act
        var result = _fileManager.ExportOptions(options);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("# OptionsPicker Export");
        result.ShouldContain("Pizza:3");
        result.ShouldContain("Burger:2");
        result.ShouldContain("Salad:1");
    }

    [TestMethod]
    public void ExportOptions_WithEmptyList_ShouldReturnHeaderOnly()
    {
        // Arrange
        var options = new List<Option>();

        // Act
        var result = _fileManager.ExportOptions(options);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("# OptionsPicker Export");
        result.Split('\n').Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
            .Count().ShouldBe(0);
    }

    [TestMethod]
    public void ExportOptions_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Café & Bakery", 1.0),
            Option.Create("Mom's Diner", 2.0),
            Option.Create("Pizza: The Best!", 1.5)
        };

        // Act
        var result = _fileManager.ExportOptions(options);

        // Assert
        result.ShouldContain("Café & Bakery:1");
        result.ShouldContain("Mom's Diner:2");
        result.ShouldContain("Pizza: The Best!:1.5");
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithValidFormat_ShouldReturnOptions()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Pizza:3
Burger:2
Salad:1";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Name.ShouldBe("Pizza");
        result[0].Weight.ShouldBe(3.0);
        result[1].Name.ShouldBe("Burger");
        result[1].Weight.ShouldBe(2.0);
        result[2].Name.ShouldBe("Salad");
        result[2].Weight.ShouldBe(1.0);
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithCommentsAndEmptyLines_ShouldIgnoreThem()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
# This is a comment
Pizza:3

Burger:2
# Another comment
Salad:1

";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Name.ShouldBe("Pizza");
        result[1].Name.ShouldBe("Burger");
        result[2].Name.ShouldBe("Salad");
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithDecimalWeights_ShouldParseCorrectly()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Option A:1.5
Option B:2.75
Option C:0.25";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Weight.ShouldBe(1.5);
        result[1].Weight.ShouldBe(2.75);
        result[2].Weight.ShouldBe(0.25);
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithMissingWeight_ShouldDefaultToOne()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Option A:2
Option B
Option C:1.5";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Weight.ShouldBe(2.0);
        result[1].Weight.ShouldBe(1.0); // Default weight
        result[2].Weight.ShouldBe(1.5);
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithInvalidWeight_ShouldThrowException()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Pizza:abc
Burger:2";

        // Act & Assert
        await Should.ThrowAsync<FormatException>(async () =>
            await _fileManager.ImportOptionsAsync(fileContent));
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithNegativeWeight_ShouldThrowException()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Pizza:-1
Burger:2";

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _fileManager.ImportOptionsAsync(fileContent));
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithZeroWeight_ShouldThrowException()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Pizza:0
Burger:2";

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _fileManager.ImportOptionsAsync(fileContent));
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithEmptyOptionName_ShouldThrowException()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
:2
Burger:1";

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _fileManager.ImportOptionsAsync(fileContent));
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithDuplicateOptions_ShouldHandleCorrectly()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Pizza:3
Burger:2
Pizza:1";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(2); // Should deduplicate by name
        var pizza = result.First(o => o.Name == "Pizza");
        pizza.Weight.ShouldBe(1.0); // Last occurrence wins
    }

    [TestMethod]
    public async Task GenerateDownloadFileAsync_WithOptions_ShouldReturnDataUrl()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Pizza", 3.0),
            Option.Create("Burger", 2.0)
        };

        // Act
        var result = await _fileManager.GenerateDownloadFileAsync(options);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldStartWith("data:text/plain;charset=utf-8,");

        // Decode and verify content
        var content = Uri.UnescapeDataString(result.Substring("data:text/plain;charset=utf-8,".Length));
        content.ShouldContain("Pizza:3");
        content.ShouldContain("Burger:2");
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithMultipleColons_ShouldHandleCorrectly()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
Time: 12:30:2
Website: http://example.com:1.5";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Time: 12:30");
        result[0].Weight.ShouldBe(2.0);
        result[1].Name.ShouldBe("Website: http://example.com");
        result[1].Weight.ShouldBe(1.5);
    }

    [TestMethod]
    public async Task ImportOptionsAsync_WithLeadingTrailingWhitespace_ShouldTrim()
    {
        // Arrange
        var fileContent = @"# OptionsPicker Export
  Pizza  : 3
  Burger:2
Salad : 1.5  ";

        // Act
        var result = await _fileManager.ImportOptionsAsync(fileContent);

        // Assert
        result.Count.ShouldBe(3);
        result[0].Name.ShouldBe("Pizza");
        result[0].Weight.ShouldBe(3.0);
        result[1].Name.ShouldBe("Burger");
        result[1].Weight.ShouldBe(2.0);
        result[2].Name.ShouldBe("Salad");
        result[2].Weight.ShouldBe(1.5);
    }
}