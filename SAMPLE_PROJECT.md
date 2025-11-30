# Sample Project Configuration

## Minimal Setup (Default Settings)

Just add the package reference and the tool will automatically generate Dart files to `generated_dart` folder after each build:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="DtoToDartGenerator" Version="1.0.0" />
  </ItemGroup>

</Project>
```

## Custom Output Directory

Generate Dart files directly into your Flutter project:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- Output to Flutter project -->
    <DartOutputDir>$(MSBuildProjectDirectory)\..\flutter_app\lib\models</DartOutputDir>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="DtoToDartGenerator" Version="1.0.0" />
  </ItemGroup>

</Project>
```

## Disable Auto-Generation

Keep the package reference but disable automatic generation (use manual command instead):

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- Disable auto-generation -->
    <GenerateDartOnBuild>false</GenerateDartOnBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="DtoToDartGenerator" Version="1.0.0" />
  </ItemGroup>

</Project>
```

Then run manually:
```bash
dto-to-dart YourProject.dll ./output
```

## Multi-Project Solution

If you have multiple DTO projects, add the package to each one:

```
Solution/
??? MyApp.Dtos/
?   ??? MyApp.Dtos.csproj          # Add package here
??? MyApp.Api.Contracts/
?   ??? MyApp.Api.Contracts.csproj # Add package here
??? MyApp.Core/
    ??? MyApp.Core.csproj          # Regular project
```

Each project with the package reference will generate its own Dart files.

## Complete Example

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- Custom output directory -->
    <DartOutputDir>$(MSBuildProjectDirectory)\..\..\mobile\lib\api\models</DartOutputDir>
    
    <!-- Optional: Disable if needed -->
    <!-- <GenerateDartOnBuild>false</GenerateDartOnBuild> -->
  </PropertyGroup>

  <ItemGroup>
    <!-- Your DTO generator package -->
    <PackageReference Include="DtoToDartGenerator" Version="1.0.0" />
    
    <!-- Other packages -->
    <PackageReference Include="System.Text.Json" Version="9.0.0" />
  </ItemGroup>

</Project>
```

Build your project and Dart files will be generated automatically! ??
