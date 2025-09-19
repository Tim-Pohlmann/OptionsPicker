# Step 7: File Import/Export

## Goals
- Define .txt file format for options
- Implement import/export functionality
- Handle file errors and validation

## Tasks

### 7.1 File Format Definition
Define simple .txt format:
```
# OptionsPicker Export
Option Name 1:5
Option Name 2:3
Option Name 3:1
# Comments and empty lines ignored
```

### 7.2 File Manager Service
- Create `FileManager` implementing `IFileManager`
- Export options to formatted text
- Parse imported files with error handling
- Validate option format and weights

### 7.3 Export Functionality
- Generate downloadable .txt file
- Include metadata (export date, option count)
- Handle special characters in option names
- Browser download integration

### 7.4 Import Functionality
- File input component for .txt files
- Parse and validate file contents
- Preview imported options before applying
- Merge vs replace existing options choice

### 7.5 Import/Export UI
- Create `ImportExport.razor` component
- Export button with file download
- Import file selector with drag-and-drop
- Progress indicators for large files
- Error messages for invalid files

### 7.6 File Validation
- Check file format and structure
- Validate option names and weights
- Handle encoding issues (UTF-8)
- Maximum file size limits
- Duplicate option handling

### 7.7 Error Handling
- User-friendly error messages
- Partial import on validation errors
- Rollback functionality
- Error reporting with line numbers

### 7.8 Advanced Features
- Multiple file format support (.json, .csv)
- Batch operations on imported options
- Template file generation
- Import from URL functionality

### 7.9 Testing
- Unit tests for file parsing
- Integration tests with file system
- Error handling tests
- Large file performance tests

## Definition of Done
- Working import/export functionality
- Robust file format parsing
- User-friendly error handling
- File validation and safety checks
- Clean UI for import/export operations
- Comprehensive test coverage
- Documentation for file format