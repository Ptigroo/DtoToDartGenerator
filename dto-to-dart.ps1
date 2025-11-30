param(
    [string]$assemblyPath = "..\SearchEngineModels\bin\Debug\net10.0\SearchEngineModels.dll",
    [string]$outDir = "..\..\search_engine_front\lib\models"
)

$exe = Join-Path $PSScriptRoot "DtoToDartGenerator\bin\Debug\net10.0\DtoToDartGenerator.exe"
if (!(Test-Path $exe)) {
    Write-Host "Building generator..."
    dotnet build "$PSScriptRoot\DtoToDartGenerator\DtoToDartGenerator.csproj"
}

& $exe $assemblyPath $outDir
