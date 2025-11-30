# DtoToDartGenerator

A .NET tool that automatically generates Dart data classes from .NET DTOs with JSON serialization support.

## Quick Start

### 1. Install the global tool
```bash
dotnet tool install -g DtoToDartGenerator
```

### 2. Add to your DTO project
```bash
dotnet add package DtoToDartGenerator
```

**That's it!** Dart files will be automatically generated after each build to the `generated_dart` folder.

## Customization

Customize the output directory in your `.csproj`:

```xml
<PropertyGroup>
  <DartOutputDir>../flutter_app/lib/models</DartOutputDir>
</PropertyGroup>
```

Disable auto-generation:

```xml
<PropertyGroup>
  <GenerateDartOnBuild>false</GenerateDartOnBuild>
</PropertyGroup>
```

## Manual Usage

Run the tool manually from command line:

```bash
# From DLL
dto-to-dart path/to/your.dll ./output

# From .csproj (will build first)
dto-to-dart YourProject.csproj ./output
```

Or use a config file `dto-to-dart.config.json`:

```json
{
  "assemblyPath": "bin/Debug/net10.0/YourProject.dll",
  "outputDir": "generated_dart"
}
```

Then run:
```bash
dto-to-dart
```

## Features

- ✅ Automatic generation on build (when installed as package reference)
- ✅ Generates Dart classes from C# DTOs
- ✅ Includes `toJson()` and `fromJson()` methods
- ✅ Supports primitive types, nullable types, and collections
- ✅ Snake_case file naming
- ✅ CamelCase property naming

## Example

**C# DTO:**
```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Hobbies { get; set; }
}
```

**Generated Dart:**
```dart
class Person {
  final String name;
  final int age;
  final List<String> hobbies;

  Person({
    required this.name,
    required this.age,
    required this.hobbies,
  });

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'age': age,
      'hobbies': hobbies,
    };
  }

  factory Person.fromJson(Map<String, dynamic> json) {
    return Person(
      name: json['name'] as String,
      age: json['age'] as int,
      hobbies: List<String>.from(json['hobbies'] as List),
    );
  }
}
```

## Full Documentation

See [USAGE.md](USAGE.md) for detailed configuration options and troubleshooting.

## License

MIT
