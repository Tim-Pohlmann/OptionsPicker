# OptionsPicker Implementation Plan

## Overview
Create a weighted options picker with spinning wheel visualization, URL-based sharing, and file import/export. Architecture designed for future alternative visualizations (marble race, etc.).

## Core Architecture

### Data Layer
- **Option Model**: Name, weight, unique ID
- **OptionCollection**: Manages options, handles weighted selection
- **URL State Manager**: Serializes/deserializes options to/from URL
- **File Manager**: Import/export .txt functionality

### Business Logic
- **Selection Engine**: Weighted random selection algorithm
- **State Management**: Central state for options and UI
- **Validation**: Option name/weight validation

### Presentation Layer
- **Visualization Interface**: Abstract base for different visualizations
- **Spinning Wheel Component**: Initial implementation
- **Options Manager Component**: Add/edit/remove options
- **Import/Export Component**: File operations

### Infrastructure
- **GitHub Pages Configuration**: Static hosting setup
- **URL Routing**: Deep linking support

## Implementation Steps

1. **Foundation & Testing Setup**
   - Add MSTest, Shouldly, NSubstitute packages
   - Create core models and interfaces
   - Set up basic project structure

2. **Core Data Models**
   - Option model with validation
   - OptionCollection with weighted selection
   - Unit tests for selection algorithm

3. **State Management**
   - Central state service
   - URL serialization/deserialization
   - State persistence logic

4. **Options Management UI**
   - Add/edit/remove options interface
   - Weight adjustment controls
   - Input validation and UX

5. **Selection Engine**
   - Weighted random selection implementation
   - Selection history (optional)
   - Animation triggers

6. **Spinning Wheel Visualization**
   - Canvas-based wheel rendering
   - Spin animation system
   - Result display

7. **File Import/Export**
   - .txt file format definition
   - Import/export functionality
   - Error handling

8. **GitHub Pages Deployment**
   - Configure for static hosting
   - Build and deployment workflow
   - URL routing for deep links

## Future Extensibility
- Visualization interface allows for marble race and other alternatives
- Modular component design for easy feature additions
- Clean separation of business logic from presentation