# Step 3: State Management

## Goals
- Implement central state service
- URL serialization/deserialization for sharing
- State persistence and change notifications

## Tasks

### 3.1 State Manager Interface & Implementation
- Create `StateManager` service implementing `IStateManager`
- Manage current options collection
- Handle state change notifications
- Register as singleton in DI container

### 3.2 URL Serialization
- Design compact URL format for options (name:weight pairs)
- Handle URL encoding/decoding
- Support for special characters in option names
- URL length limitations and compression

### 3.3 State Persistence
- Serialize/deserialize options to/from URL parameters
- Update browser URL without page reload
- Handle malformed URLs gracefully
- Default state when no URL parameters

### 3.4 Change Notifications
- Implement INotifyPropertyChanged or similar
- Notify UI components of state changes
- Batch updates to prevent excessive re-renders

### 3.5 URL Integration Tests
Create tests for:
- Round-trip serialization (options → URL → options)
- Edge cases (empty options, special characters)
- URL length handling
- Browser navigation integration

### 3.6 Blazor Integration
- Hook into Blazor's NavigationManager
- Update URL on state changes
- Load state from URL on app startup

## Definition of Done
- StateManager service working with DI
- Options serializable to/from URL
- Browser URL updates with state changes
- URL sharing works correctly
- All edge cases handled gracefully
- Integration tests passing