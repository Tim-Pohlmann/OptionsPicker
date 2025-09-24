# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Blazor WebAssembly application targeting .NET 10.0. The project is named "OptionsPicker" and is designed as an options picker or decision-making tool.

## Architecture

- **Framework**: Blazor WebAssembly using .NET 10.0
- **Root namespace**: `OptionsPicker`
- **Entry point**: `Program.cs` - standard Blazor WebAssembly bootstrapping
- **Routing**: Handled by `App.razor` with `Router` component
- **Layout**: Simple `MainLayout.razor` with minimal structure
- **Pages**: Currently only has a basic `Index.razor` page at root route "/"

## Development Commands

### Build and Run
```bash
dotnet build
dotnet run
```

### Development Server
The application runs on:
- HTTP: http://localhost:5251
- HTTPS: https://localhost:7055

### Project Structure
- **Pages/**: Razor pages with routing
- **wwwroot/**: Static web assets (CSS, HTML)
- **_Imports.razor**: Global using statements for Razor components
- **MainLayout.razor**: Main application layout
- **App.razor**: Application root with routing configuration

## Key Dependencies
- Microsoft.AspNetCore.Components.WebAssembly (v7.0.20)
- Microsoft.AspNetCore.Components.WebAssembly.DevServer (v7.0.20)

## Claude Code Workflow

### Planning and Execution Process
1. **Generate Plan.md** - Create high-level task breakdown
2. **Create Instructions/** folder with detailed step-by-step files
3. **Use TodoWrite tool** to track progress through each instruction file
4. **Execute systematically** - work through each instruction file in order
5. **Commit after each step** - Create a git commit when completing each instruction file

### Communication Style
- **Be concise** in messages and code
- **Ask for clarifications** rather than making assumptions
- **Reference specific files/lines** using `file_path:line_number` format

### Development Standards
- **🧪 TEST-DRIVEN DEVELOPMENT IS MANDATORY**
  - **ALWAYS write tests BEFORE implementing new features**
  - **NEVER commit code without corresponding tests**
  - **Every new service, component, or business logic MUST have tests**
- **Modern C#** - Use latest language features and patterns
- **Testing Framework**: MSTest with Shouldly assertions and NSubstitute for mocking

### TDD Workflow (FOLLOW STRICTLY)
1. **Write failing test first** - Red
2. **Write minimal code to pass** - Green
3. **Refactor while keeping tests green** - Refactor
4. **Commit both test and implementation together**

### Test Commands
```bash
dotnet test
```

## Development Notes

The project is currently in a minimal state with basic Blazor WebAssembly scaffolding. The main page displays "Hello, world!" and is ready for development of the options picker functionality.