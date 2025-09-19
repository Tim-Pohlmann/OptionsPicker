# Step 5: Selection Engine

## Goals
- Implement weighted random selection with UI integration
- Add selection history and animation triggers
- Create the core "pick an option" functionality

## Tasks

### 5.1 Selection Service
- Create `SelectionService` implementing selection logic
- Integrate with OptionCollection for weighted selection
- Return SelectionResult with metadata (timestamp, etc.)
- Handle edge cases (no options, all zero weights)

### 5.2 Selection History
- Track recent selections (last 10-20)
- Display selection history in UI
- Option to clear history
- Prevent immediate consecutive duplicates (optional setting)

### 5.3 Animation Integration
- Create selection events/callbacks for UI animations
- Async selection method for animation timing
- Selection state management (selecting, complete)

### 5.4 Selection Controls UI
- Large "SPIN" or "PICK" button
- Display current selection prominently
- Show selection in progress state
- History panel component

### 5.5 Selection Statistics
- Track selection counts per option
- Display fairness metrics (expected vs actual)
- Reset statistics functionality

### 5.6 Advanced Features
- Multiple selections at once
- Exclude previous winner option
- Selection with replacement toggle

### 5.7 Testing
- Unit tests for selection service
- Statistical tests for fairness over many selections
- UI component tests for selection flow
- Integration tests with state management

## Definition of Done
- Working selection engine with weighted randomization
- Selection history tracking and display
- Clean UI for triggering selections
- Animation hooks ready for visualization components
- Statistical fairness validated through testing
- All edge cases handled gracefully