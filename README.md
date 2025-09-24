# Options Picker 🎯

A modern web application built with Blazor WebAssembly for making random decisions with customizable weighted options and a beautiful spinning wheel visualization.

## 🌟 Features

- **Weighted Random Selection**: Create options with different weights to influence selection probability
- **Interactive Spinning Wheel**: Beautiful canvas-based wheel visualization with smooth animations
- **Import/Export**: Save and load your option lists as text files
- **URL Sharing**: Share your options with others via URL parameters
- **Selection History**: Track past selections and view statistics
- **Responsive Design**: Works perfectly on desktop and mobile devices
- **Real-time Updates**: Live preview of option changes and weight adjustments

## 🚀 Live Demo

Visit the live application: [Options Picker on GitHub Pages](https://yourusername.github.io/OptionsPicker/)

## 📱 Usage

1. **Add Options**: Click "Add Option" to create new choices with custom weights
2. **Adjust Weights**: Higher weights make options more likely to be selected
3. **Spin the Wheel**: Click the spinning wheel or "Spin the Wheel!" button
4. **View Results**: See the selected option and browse your selection history
5. **Share Options**: Copy the URL to share your option list with others
6. **Import/Export**: Save your options to a file or load from a previous export

## 🛠️ Technical Details

### Built With
- **Framework**: Blazor WebAssembly (.NET 10.0)
- **UI**: Modern CSS with responsive design
- **Visualization**: HTML5 Canvas with JavaScript interop
- **Testing**: MSTest, Shouldly, NSubstitute, bUnit
- **Deployment**: GitHub Pages with automated CI/CD

### Architecture
- **Data Layer**: Immutable Option models with validation
- **Business Logic**: Weighted selection algorithms and state management
- **Presentation Layer**: Blazor components with scoped CSS
- **Services**: Dependency injection with clean interfaces

## 🔧 Development Setup

### Prerequisites
- .NET 10.0 SDK (preview)
- Git

### Running Locally
```bash
# Clone the repository
git clone https://github.com/yourusername/OptionsPicker.git
cd OptionsPicker

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project src/OptionsPicker

# Run tests
dotnet test
```

The application will be available at:
- HTTP: http://localhost:5251
- HTTPS: https://localhost:7055

### Building for Production
```bash
# Build optimized release version
dotnet publish src/OptionsPicker/OptionsPicker.csproj --configuration Release --output publish
```

## 📁 File Format

The application supports importing and exporting options in a simple text format:

```
# OptionsPicker Export
# Generated on 2025-01-24 15:30:00 UTC
# Total options: 3

Pizza:3
Burger:2
Salad:1.5
# Comments and empty lines are ignored
```

### Format Rules
- Each option on a new line: `Name:Weight`
- Weight is optional (defaults to 1.0)
- Lines starting with `#` are comments
- Empty lines are ignored
- Special characters in names are supported

## 🧪 Testing

The project includes comprehensive test coverage:
- **86 total tests** covering all major functionality
- **Unit Tests**: Core business logic and data models
- **Integration Tests**: Service interactions and workflows
- **Component Tests**: Blazor UI components with bUnit
- **File I/O Tests**: Import/export functionality

```bash
# Run all tests
dotnet test

# Run tests with coverage (requires tools)
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 Deployment

The project is configured for automatic deployment to GitHub Pages using GitHub Actions.

### Automatic Deployment
1. Push changes to the `main` branch
2. GitHub Actions automatically builds and tests the application
3. If tests pass, the app is deployed to GitHub Pages
4. The live site is updated within minutes

### Manual Deployment
1. Enable GitHub Pages in your repository settings
2. Set source to "GitHub Actions"
3. Push changes to trigger the deployment workflow

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes following the existing code style
4. Add tests for new functionality
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

## 📋 Project Structure

```
OptionsPicker/
├── src/OptionsPicker/           # Main application
│   ├── Components/              # Blazor components
│   ├── Models/                  # Data models
│   ├── Pages/                   # Razor pages
│   ├── Services/                # Business logic services
│   └── wwwroot/                 # Static assets
├── Tests/OptionsPicker/         # Test project
├── Instructions/                # Implementation steps
├── .github/workflows/           # CI/CD configuration
└── README.md                    # This file
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with love using Blazor WebAssembly
- Icons and emojis for enhanced user experience
- Community feedback and contributions

---

Made with ❤️ by [Your Name] | Deployed with GitHub Pages