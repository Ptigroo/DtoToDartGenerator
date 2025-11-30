# NuGet Package Publishing Checklist

## ? Files Created/Modified

### 1. **DtoToDartGenerator.csproj** - Updated
   - Added NuGet package metadata
   - Set `PackAsTool=true` for global tool installation
   - Included MSBuild targets file in package
   - Added `DevelopmentDependency=true`

### 2. **build/DtoToDartGenerator.targets** - New
   - Automatic MSBuild integration
   - Runs after build automatically
   - Configurable via MSBuild properties:
     - `DartOutputDir` - Customize output location
     - `GenerateDartOnBuild` - Enable/disable auto-generation

### 3. **README.md** - Updated
   - Quick start instructions
   - Installation guide
   - Usage examples

### 4. **USAGE.md** - New
   - Detailed usage guide
   - Customization options
   - Troubleshooting

### 5. **SAMPLE_PROJECT.md** - New
   - Ready-to-copy `.csproj` examples
   - Various configuration scenarios

## ?? Publishing Steps

### Before Publishing:
1. **Update metadata in DtoToDartGenerator.csproj:**
   ```xml
   <Authors>Your Name</Authors>
   <Company>Your Company</Company>
   <Version>1.0.0</Version>
   ```

2. **Test locally:**
   ```bash
   cd DtoToDartGenerator
   dotnet pack -c Release
   dotnet tool install -g --add-source ./bin/Release DtoToDartGenerator
   ```

3. **Test in a sample project:**
   ```bash
   cd ../TestProject
   dotnet add package DtoToDartGenerator --source ../DtoToDartGenerator/bin/Release
   dotnet build
   ```

### Publishing to NuGet.org:

1. **Get API Key:**
   - Go to https://www.nuget.org/
   - Sign in / Create account
   - Go to Account ? API Keys
   - Create new key with push permissions

2. **Build and pack:**
   ```bash
   cd DtoToDartGenerator
   dotnet pack -c Release
   ```

3. **Push to NuGet:**
   ```bash
   dotnet nuget push bin/Release/DtoToDartGenerator.1.0.0.nupkg \
     --api-key YOUR_API_KEY \
     --source https://api.nuget.org/v3/index.json
   ```

4. **Wait for indexing:**
   - Takes 10-15 minutes for package to appear
   - Check status at https://www.nuget.org/packages/DtoToDartGenerator

## ?? User Experience

### For End Users:

1. **Install global tool (one time):**
   ```bash
   dotnet tool install -g DtoToDartGenerator
   ```

2. **Add to their DTO project:**
   ```bash
   dotnet add package DtoToDartGenerator
   ```

3. **Optional: Customize in .csproj:**
   ```xml
   <PropertyGroup>
     <DartOutputDir>../flutter_app/lib/models</DartOutputDir>
   </PropertyGroup>
   ```

4. **Build and done!**
   ```bash
   dotnet build
   ```
   Dart files generated automatically! ?

### Key Benefits:
- ? **No manual Target copy-paste** - MSBuild targets included in package
- ? **Zero configuration** - Works out of the box with sensible defaults
- ? **Customizable** - Easy property overrides
- ? **Clean** - Development dependency, doesn't pollute runtime deps

## ?? Version Management

Update version in `.csproj` before each release:
```xml
<Version>1.0.1</Version>
```

Consider semantic versioning:
- `1.0.0` - Initial release
- `1.0.1` - Bug fixes
- `1.1.0` - New features (backward compatible)
- `2.0.0` - Breaking changes

## ?? Next Steps

1. Update author info in `.csproj`
2. Test locally
3. Create NuGet.org account
4. Publish version 1.0.0
5. Share package ID with users: `DtoToDartGenerator`
