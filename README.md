# DtoToDartGenerator

A .NET tool that automatically generates Dart data classes from .NET DTOs with JSON serialization support.

## Quick Start

### 1. Install the global tool
```bash
dotnet tool install -g DtoToDartGenerator
```

### 2. Add to your DTO class library
```bash
dotnet add package DtoToDartGenerator
```

Dart files will be automatically generated after each build to the `generated_dart` folder.

## Customization preferred

Customize the output directory in your `.csproj` using relative path vy adding  <DartOutputDir> in your propertyGroup:

```xml
<PropertyGroup>
    <DartOutputDir>..\..\..\search_engine_front\lib\models</DartOutputDir>
</PropertyGroup>
```

Disable auto-generation:

```xml
<PropertyGroup>
  <GenerateDartOnBuild>false</GenerateDartOnBuild>
</PropertyGroup>
```

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
