using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
namespace DtoToDartGenerator
{
    internal static class Program
    {
        private class Config
        {
            public string? AssemblyPath { get; set; }
            public string? OutputDir { get; set; }
        }
        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                var currentDir = Directory.GetCurrentDirectory();
                string[] possibleConfigPaths = new[]
                {
                    Path.Combine("..",  "DtoToDartGenerator", "dto-to-dart.config.json"),
                };

                string? configPath = null;
                foreach (var path in possibleConfigPaths)
                {
                    var fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        configPath = fullPath;
                        break;
                    }
                }
                if (configPath != null)
                {
                    try
                    {
                        var json = File.ReadAllText(configPath);
                        var cfg = JsonSerializer.Deserialize<Config>(json);
                        if (!string.IsNullOrWhiteSpace(cfg?.AssemblyPath) && !string.IsNullOrWhiteSpace(cfg?.OutputDir))
                        {
                            // resolve relative assembly path against config file directory
                            var cfgDir = Path.GetDirectoryName(configPath) ?? currentDir;
                            var assemblyPath = Path.GetFullPath(Path.Combine(cfg.AssemblyPath));
                            var outputDir = Path.GetFullPath(Path.Combine(cfgDir, cfg.OutputDir));
                            args = new[] { assemblyPath, outputDir };
                        }
                        else
                        {
                            Console.WriteLine("Assembly: " + cfg?.AssemblyPath ?? "");
                            Console.WriteLine("Out: " + cfg?.OutputDir ?? "");
                            Console.WriteLine("Config file found but missing fields 'assemblyPath' or 'outputDir'. Provide args or fix config.");
                            return 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to read config: " + ex.Message);
                        return 1;
                    }
                }
                else
                {
                    Console.WriteLine("No args provided and config file not found at: " + string.Join(", ", possibleConfigPaths));
                    Console.WriteLine("Usage: DtoToDartGenerator <path-to-dll-or-csproj> <output-folder> or provide tools/dto-to-dart.config.json");
                    return 1;
                }
            }
            var dllArg = args[0]?.Trim('"') ?? string.Empty;
            var outDir = args[1];
            if (Path.GetExtension(dllArg).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                var projPath = dllArg;
                Console.WriteLine("Building project: " + projPath);
                var psi = new ProcessStartInfo("dotnet", "build \"" + projPath + "\" -c Debug")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
                    proc.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                    {
                        Console.WriteLine("dotnet build failed for project: " + projPath);
                        return 1;
                    }
                }

                var projDir = Path.GetDirectoryName(Path.GetFullPath(projPath)) ?? Directory.GetCurrentDirectory();
                var binDebug = Path.Combine(projDir, "bin", "Debug");
                if (Directory.Exists(binDebug))
                {
                    // Find the most recent framework folder
                    var fxDirs = Directory.GetDirectories(binDebug, "net*");
                    if (fxDirs.Length > 0)
                    {
                        var candidateDir = fxDirs.OrderByDescending(d => d).First();
                        var dlls = Directory.GetFiles(candidateDir, "*.dll");
                        if (dlls.Length > 0)
                        {
                            dllArg = dlls[0];
                            Console.WriteLine("Using built assembly: " + dllArg);
                        }
                    }
                }
            }
            var dll = dllArg;
            if (!File.Exists(dll))
            {
                Console.WriteLine($"Assembly not found: {dll}");
                Console.WriteLine("Please provide a valid path to the assembly or update dto-to-dart.config.json");
                return 2;
            }

            Directory.CreateDirectory(outDir);

            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(dll);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load assembly: " + ex.Message);
                return 3;
            }

            var types = asm.GetTypes().Where(t => t.IsClass && t.IsPublic && t.Namespace != null && t.Name.EndsWith("Dto"));

            foreach (var type in types)
            {
                var sb = new StringBuilder();
                var className = type.Name;
                sb.AppendLine("class " + className + " {");

                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in props)
                {
                    var dartType = MapType(p.PropertyType);
                    sb.AppendLine("  final " + dartType + " " + ToCamelCase(p.Name) + ";");
                }

                // constructor
                sb.AppendLine();
                sb.AppendLine("  " + className + "({");
                foreach (var p in props)
                {
                    sb.AppendLine("    required this." + ToCamelCase(p.Name) + ",");
                }
                sb.AppendLine("  });");

                // toJson
                sb.AppendLine();
                sb.AppendLine("  Map<String, dynamic> toJson() {\n    return {");
                foreach (var p in props)
                {
                    sb.AppendLine("      '" + p.Name.ToLower() + "': " + ToCamelCase(p.Name) + ",");
                }
                sb.AppendLine("    };\n  }");

                // fromJson
                sb.AppendLine();
                sb.AppendLine("  factory " + className + ".fromJson(Map<String, dynamic> json) {");
                sb.AppendLine("    return " + className + "(");
                foreach (var p in props)
                {
                    var parse = FromJsonParse(p.PropertyType, "json['" + p.Name.ToLower() + "']");
                    sb.AppendLine("      " + ToCamelCase(p.Name) + ": " + parse + ",");
                }
                sb.AppendLine("    );");
                sb.AppendLine("  }");

                sb.AppendLine("}");

                var fileName = Path.Combine(outDir, ToSnakeCase(className) + ".dart");
                File.WriteAllText(fileName, sb.ToString());
                Console.WriteLine($"Wrote {fileName}");
            }

            return 0;
        }

        private static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        private static string ToSnakeCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (char.IsUpper(c) && i > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }

        private static string MapType(Type t)
        {
            if (t == typeof(string)) return "String";
            if (t == typeof(int)) return "int";
            if (t == typeof(long)) return "int";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(double) || t == typeof(float) || t == typeof(decimal)) return "double";
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var inner = t.GetGenericArguments()[0];
                return MapType(inner) + "?";
            }
            if (t.IsArray) return "List<" + MapType(t.GetElementType()!) + ">";
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
            {
                var inner = t.GetGenericArguments()[0];
                return "List<" + MapType(inner) + ">";
            }
            return "dynamic";
        }

        private static string FromJsonParse(Type t, string jsonExpr)
        {
            if (t == typeof(string)) return jsonExpr + " as String";
            if (t == typeof(int)) return jsonExpr + " as int";
            if (t == typeof(long)) return jsonExpr + " as int";
            if (t == typeof(bool)) return jsonExpr + " as bool";
            if (t == typeof(double)) return "(" + jsonExpr + " as num).toDouble()";
            if (t == typeof(float) || t == typeof(decimal)) return "(" + jsonExpr + " as num).toDouble()";
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var inner = t.GetGenericArguments()[0];
                var innerParse = FromJsonParse(inner, jsonExpr);
                return innerParse;
            }
            if (t.IsArray)
            {
                var el = MapType(t.GetElementType()!);
                return $"List<{el}>.from({jsonExpr} as List)";
            }
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
            {
                var inner = MapType(t.GetGenericArguments()[0]);
                return $"List<{inner}>.from({jsonExpr} as List)";
            }
            return jsonExpr;
        }
    }
}