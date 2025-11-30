# DtoToDartGenerator Usage Guide

## Quick Start (Automatic)

### Step 1: Install as a global tool
```bash
dotnet tool install -g DtoToDartGenerator
```

### Step 2: Add package reference to your DTO project
```bash
cd YourDtoProject
dotnet add package DtoToDartGenerator
```

That's it! The Dart files will be automatically generated after each build to `generated_dart` folder.

## Customization

### Change Output Directory

Add this to your `.csproj` file:

```xml
<PropertyGroup>
  <DartOutputDir>$(MSBuildProjectDirectory)\custom_output</DartOutputDir>
</PropertyGroup>
```

### Disable Auto-Generation

Add this to your `.csproj` file:

```xml
<PropertyGroup>
  <GenerateDartOnBuild>false</GenerateDartOnBuild>
</PropertyGroup>
```

### Example Custom Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    
    <!-- Customize Dart output location -->
    <DartOutputDir>../flutter_app/lib/models</DartOutputDir>
    
    <!-- Optional: Disable auto-generation -->
    <!-- <GenerateDartOnBuild>false</GenerateDartOnBuild> -->
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="DtoToDartGenerator" Version="1.0.0" />
  </ItemGroup>
  
</Project>
```

## Manual Usage

You can also run the tool manually:

```bash
# Using global tool
dto-to-dart YourProject.dll ./output

# Or with .csproj (will build first)
dto-to-dart YourProject.csproj ./output
```

## Configuration File

Alternatively, create a `dto-to-dart.config.json`:

```json
{
  "assemblyPath": "bin/Debug/net10.0/YourProject.dll",
  "outputDir": "generated_dart"
}
```

Then run without arguments:
```bash
dto-to-dart
```

## Troubleshooting

### Tool not found after installation
Ensure your PATH includes the .NET tools directory:
- Windows: `%USERPROFILE%\.dotnet\tools`
- macOS/Linux: `$HOME/.dotnet/tools`

Or reinstall:
```bash
dotnet tool uninstall -g DtoToDartGenerator
dotnet tool install -g DtoToDartGenerator
```

### Generation not running automatically
1. Ensure you've installed the tool globally: `dotnet tool install -g DtoToDartGenerator`
2. Check that your project builds successfully
3. Verify the package reference exists in your `.csproj`
4. Clean and rebuild: `dotnet clean && dotnet build`

### Custom output directory not working
Use absolute paths or paths relative to `$(MSBuildProjectDirectory)`:
```xml
<DartOutputDir>$(MSBuildProjectDirectory)\..\flutter_app\lib\models</DartOutputDir>
```
