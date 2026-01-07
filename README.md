[![NuGet Version](https://img.shields.io/nuget/v/xiSage.GodotResourceUID)](https://www.nuget.org/packages/xiSage.GodotResourceUID)

# GodotResourceUID

A C# library for handling Godot's Unique ID (UID) system, providing conversion between UID text format and numeric IDs, as well as UID-to-path mapping functionality.

## Features

- **UID Conversion**: Convert between UID text format (`uid://d4n4ub6itg400`) and numeric IDs
- **Cache Management**: Load UID mappings from binary cache files
- **Path Mapping**: Map between UIDs and file paths
- **Godot Compatible**: Implements Godot's UID character mapping and format specification
- **Thread Safe**: Designed for safe use in multi-threaded environments
- **Unit Tested**: Comprehensive test suite ensuring reliable functionality

## Installation

### Prerequisites

- .NET 6.0 or later
- Godot 4.x (for Godot projects)

### Usage in .NET Projects

1. Clone the repository:
   ```bash
   git clone https://github.com/xiSage/GodotResourceUID.git
   ```

2. Add the project reference to your .NET project:
   ```xml
   <ProjectReference Include="path/to/GodotResourceUID/GodotResourceUID/GodotResourceUID.csproj" />
   ```

3. Add the namespace to your code:
   ```csharp
   using GodotResourceUID;
   ```

### Usage in Godot Projects

1. Copy the `ResourceUID.cs` file to your Godot project's `addons` or `scripts` directory
2. Add the namespace to your GDScript or C# files

## API Reference

### ResourceUID Class

#### Constants

- `INVALID_ID`: Constant representing an invalid UID (-1)

#### Static Methods

- **`string IdToText(long id)`**
  Converts a numeric UID to its text format.
  - Parameters: `id` - Numeric UID to convert
  - Returns: Text format UID (e.g., `uid://d4n4ub6itg400`)

- **`long TextToId(string text)`**
  Converts a text format UID to its numeric representation.
  - Parameters: `text` - Text format UID to convert
  - Returns: Numeric UID, or `INVALID_ID` if invalid

- **`string GetPathFromCache(string cacheFilePath, string uidString)`**
  Gets a path from a cache file directly by UID text, without loading the entire cache.
  - Parameters: 
    - `cacheFilePath` - Path to the UID cache file
    - `uidString` - Text format UID to look up
  - Returns: Corresponding file path, or empty string if not found

#### Instance Methods

- **`bool LoadFromCache(string cacheFilePath)`**
  Loads UID mapping from a cache file.
  - Parameters: `cacheFilePath` - Path to the UID cache file
  - Returns: `true` if loading was successful, `false` otherwise

- **`string GetIdPath(long id)`**
  Gets the path associated with a numeric UID.
  - Parameters: `id` - Numeric UID to look up
  - Returns: Corresponding file path, or empty string if not found

- **`long GetPathId(string path)`**
  Gets the numeric UID associated with a file path.
  - Parameters: `path` - File path to look up
  - Returns: Corresponding numeric UID, or `INVALID_ID` if not found

- **`string UidToPath(string uid)`**
  Converts a text format UID to its corresponding file path.
  - Parameters: `uid` - Text format UID to convert
  - Returns: Corresponding file path, or empty string if not found

- **`string EnsurePath(string pathOrUid)`**
  Ensures a path is returned, converting to path if input is a UID.
  - Parameters: `pathOrUid` - File path or text format UID
  - Returns: File path

## Usage Examples

### Basic UID Conversion

```csharp
using GodotResourceUID;

// Convert numeric ID to text format
long numericId = 0x7FFFFFFFFFFFFFFF;
string textUid = ResourceUID.IdToText(numericId);
// textUid will be "uid://d4n4ub6itg400"

// Convert text format to numeric ID
long convertedId = ResourceUID.TextToId(textUid);
// convertedId will be 0x7FFFFFFFFFFFFFFF
```

### Working with UID Cache

```csharp
using GodotResourceUID;

ResourceUID uidManager = new ResourceUID();

// Load UID mappings from cache file
if (uidManager.LoadFromCache("res://.godot/uid_cache.bin"))
{
    // Get path from UID
    string texturePath = uidManager.UidToPath("uid://d4n4ub6itg400");
    // texturePath will be something like "res://textures/player.png"
    
    // Get UID from path
    string modelPath = "res://models/enemy.tscn";
    long modelUid = uidManager.GetPathId(modelPath);
    // modelUid will be the numeric UID for the model
}
```

### Ensuring Paths

```csharp
using GodotResourceUID;

ResourceUID uidManager = new ResourceUID();
uidManager.LoadFromCache("res://.godot/uid_cache.bin");

// This works with both paths and UIDs
string path1 = uidManager.EnsurePath("res://scenes/level.tscn");
// path1 remains "res://scenes/level.tscn"

string path2 = uidManager.EnsurePath("uid://dm3rdgs30kfci");
// path2 becomes the actual file path
```

## Testing

The project includes a comprehensive test suite using xUnit. To run the tests:

1. Navigate to the project directory
2. Run the test command:
   ```bash
   dotnet test
   ```

## Architecture

### UID Format

Godot UIDs use a base-34 encoding with the following character set:
- Lowercase letters: `a-z` (26 characters)
- Digits: `0-9` (10 characters)

Total: 34 characters

### Cache Format

The UID cache file is a binary file with the following structure:
1. 4 bytes: Number of entries (uint32)
2. For each entry:
   - 8 bytes: Numeric UID (int64)
   - 4 bytes: Path length (int32)
   - Variable: UTF-8 encoded path

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Acknowledgments

- Based on Godot Engine's UID implementation
- Inspired by the need for UID handling in .NET applications working with Godot projects

## Contact

For issues, questions, or suggestions, please open an issue on GitHub.
