# Step 1: Foundation & Testing Setup

## Goals
- Add testing packages (MSTest, Shouldly, NSubstitute)
- Create basic project structure
- Set up core models and interfaces

## Tasks

### 1.1 Add Testing Packages
```bash
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package MSTest.TestAdapter
dotnet add package MSTest.TestFramework
dotnet add package Shouldly
dotnet add package NSubstitute
```

### 1.2 Create Test Project Structure
- Create `Tests/` folder
- Create `Models/` folder
- Create `Services/` folder
- Create `Components/` folder

### 1.3 Core Interfaces
Create interfaces in `Services/`:
- `IOptionCollection` - manages options and selection
- `IVisualizationEngine` - abstract visualization interface
- `IStateManager` - handles URL and app state
- `IFileManager` - import/export operations

### 1.4 Basic Models
Create in `Models/`:
- `Option` record with Name, Weight, Id
- `SelectionResult` record with selected option and metadata

### 1.5 First Unit Tests
Create basic test structure and first tests for:
- Option model validation
- Basic collection operations

## Definition of Done
- All packages installed
- Folder structure created
- Core interfaces defined
- Basic models with validation
- First unit tests passing
- `dotnet test` command works