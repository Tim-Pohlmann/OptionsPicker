using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using NSubstitute;
using OptionsPicker.Models;
using OptionsPicker.Components;
using OptionsPicker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Bunit;

namespace OptionsPicker.Tests;

[TestClass]
public class ImportExportTests : TestContextWrapper
{
    private IFileManager _mockFileManager = null!;
    private IStateManager _mockStateManager = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockFileManager = Substitute.For<IFileManager>();
        _mockStateManager = Substitute.For<IStateManager>();

        TestContext.Services.AddSingleton(_mockFileManager);
        TestContext.Services.AddSingleton(_mockStateManager);

        // Setup JavaScript module mock
        var mockJSModule = Substitute.For<IJSObjectReference>();
        TestContext.JSInterop.SetupModule("./js/file-utils.js").SetupVoid("downloadFile");
        TestContext.JSInterop.SetupModule("./js/file-utils.js").Setup<string>("readFile");
    }

    [TestMethod]
    public void ImportExport_WithNoOptions_ShouldShowEmptyState()
    {
        // Arrange
        _mockStateManager.CurrentOptions.Returns(new List<Option>());

        // Act
        var component = TestContext.RenderComponent<ImportExport>();

        // Assert
        var exportButton = component.Find(".export-button");
        exportButton.HasAttribute("disabled").ShouldBeTrue();

        var container = component.Find(".import-export-container");
        container.ShouldNotBeNull();
    }

    [TestMethod]
    public void ImportExport_WithOptions_ShouldEnableExport()
    {
        // Arrange
        var options = new List<Option>
        {
            Option.Create("Pizza", 3.0),
            Option.Create("Burger", 2.0)
        };
        _mockStateManager.CurrentOptions.Returns(options);

        // Act
        var component = TestContext.RenderComponent<ImportExport>();

        // Assert
        var exportButton = component.Find(".export-button");
        exportButton.HasAttribute("disabled").ShouldBeFalse();
        exportButton.TextContent.ShouldContain("Export");
    }

    [TestMethod]
    public void ImportExport_ShouldShowFileInput()
    {
        // Arrange
        _mockStateManager.CurrentOptions.Returns(new List<Option>());

        // Act
        var component = TestContext.RenderComponent<ImportExport>();

        // Assert
        var fileInput = component.Find("input[type='file']");
        fileInput.ShouldNotBeNull();
        fileInput.GetAttribute("accept").ShouldContain(".txt");
    }

    [TestMethod]
    public void ImportExport_ShouldHaveImportExportSection()
    {
        // Arrange
        _mockStateManager.CurrentOptions.Returns(new List<Option>());

        // Act
        var component = TestContext.RenderComponent<ImportExport>();

        // Assert
        var section = component.Find(".import-section");
        section.ShouldNotBeNull();

        var exportSection = component.Find(".export-section");
        exportSection.ShouldNotBeNull();
    }

    [TestMethod]
    public void ImportExport_ShouldShowInstructions()
    {
        // Arrange
        _mockStateManager.CurrentOptions.Returns(new List<Option>());

        // Act
        var component = TestContext.RenderComponent<ImportExport>();

        // Assert
        var instructions = component.FindAll(".file-format-info");
        instructions.Count.ShouldBeGreaterThan(0);
    }
}