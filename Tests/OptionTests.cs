using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using OptionsPicker.Models;

namespace OptionsPicker.Tests;

[TestClass]
public class OptionTests
{
    [TestMethod]
    public void Create_WithValidNameAndWeight_ShouldReturnOption()
    {
        var option = Option.Create("Test Option", 2.5);
        
        option.Name.ShouldBe("Test Option");
        option.Weight.ShouldBe(2.5);
        option.Id.ShouldNotBe(Guid.Empty);
    }
    
    [TestMethod]
    public void Create_WithDefaultWeight_ShouldUseWeightOfOne()
    {
        var option = Option.Create("Test Option");
        
        option.Weight.ShouldBe(1.0);
    }
    
    [TestMethod]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => Option.Create(""))
            .Message.ShouldContain("Option name cannot be empty");
    }
    
    [TestMethod]
    public void Create_WithWhitespaceName_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => Option.Create("   "))
            .Message.ShouldContain("Option name cannot be empty");
    }
    
    [TestMethod]
    public void Create_WithZeroWeight_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => Option.Create("Test", 0))
            .Message.ShouldContain("Weight must be greater than 0");
    }
    
    [TestMethod]
    public void Create_WithNegativeWeight_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => Option.Create("Test", -1))
            .Message.ShouldContain("Weight must be greater than 0");
    }
    
    [TestMethod]
    public void Create_WithNameContainingWhitespace_ShouldTrimWhitespace()
    {
        var option = Option.Create("  Test Option  ");
        
        option.Name.ShouldBe("Test Option");
    }
    
    [TestMethod]
    public void Options_WithSameContent_ShouldBeEqual()
    {
        var option1 = new Option { Name = "Test", Weight = 1.0 };
        var option2 = new Option { Name = "Test", Weight = 1.0 };
        
        option1.ShouldNotBe(option2); // Different IDs
        option1.Name.ShouldBe(option2.Name);
        option1.Weight.ShouldBe(option2.Weight);
    }
}