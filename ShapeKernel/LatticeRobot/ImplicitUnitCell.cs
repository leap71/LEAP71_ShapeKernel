using System.Numerics;
using System.Collections.Generic;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics.Tracing;
using PicoGK;
using g4;

public class ImplicitUnitCell : IImplicit {
    private FieldInfo latticeIndexField;
    Type unitCellType;
    MethodInfo valueMethod;

    public Dictionary<string, ImplicitParameter> Parameters { get; private set; }

    const string sourcePath = @"..\..\..\LEAP71_ShapeKernel\ShapeKernel\LatticeRobot";

    public ImplicitUnitCell(string codeRepPath, int latticeIndex) {
        Console.WriteLine($"Using unit cell {codeRepPath}.");

        var manifestContent = ReadText(Path.Combine(codeRepPath, "manifest.json"));
        var manifest = JsonConvert.DeserializeObject<ImplicitManifest>(manifestContent);

        if (manifest is null)
            throw new Exception("Failed to parse manifest JSON.");

        Parameters = manifest.parameters.ToDictionary(p => p.name, p => p);

        var sources = new string[] {
            Path.Combine(sourcePath, "Implicit.cs"),
            Path.Combine(sourcePath, "ImplicitParameter.cs"),
            Path.Combine(codeRepPath, manifest.cSharpLibrary),
            Path.Combine(codeRepPath, manifest.cSharpCode)
        };

        var codeList = sources.Select(s => ReadText(s));

        unitCellType = BuildImplicit(codeList);
        if (unitCellType == null) 
            throw new Exception("Error compiling unit cell.");

        var latticeIndexField = unitCellType.GetField("VariantIndex");
        if (latticeIndexField is null)
            throw new Exception("Error getting VariantIndex field.");

        foreach (var p in Parameters.Values) {
            SetParameter(p.name, p.defaultValue);
        }

        valueMethod = unitCellType.GetMethod("Value");
        if (valueMethod is null)
            throw new Exception("Error getting value callback.");

    }

    public int VariantIndex {
        get => (int)latticeIndexField.GetValue(null);
        set => latticeIndexField.SetValue(null, value);
    }

    public void SetParameter(string name, double value) {
        Parameters[name].value = value;
        var param = unitCellType.GetField(name);
        if (param is FieldInfo)
            param.SetValue(null, value);
    }

    public float fSignedDistance(in Vector3 p) {
        var g4p = new Vector3d(p.X, p.Y, p.Z);
        return (float)(double)valueMethod.Invoke(null, new object[] { g4p });
    }

    public BBox3 Bounds {
        get {
            var halfsize = new Vector3((float)Parameters["size_x"].defaultValue, (float)Parameters["size_y"].defaultValue, (float)Parameters["size_z"].defaultValue) * 0.5f;
            return new BBox3(-halfsize, halfsize);
        }
    }

    private static Type BuildImplicit(IEnumerable<string> sources) {
        // based on
        // https://stackoverflow.com/questions/32769630/how-to-compile-a-c-sharp-file-with-roslyn-programmatically
        // https://weblog.west-wind.com/posts/2022/Jun/07/Runtime-CSharp-Code-Compilation-Revisited-for-Roslyn

        var syntaxTrees = sources.Select(s => CSharpSyntaxTree.ParseText(s)).ToArray();
        var rtPath = Path.GetDirectoryName(typeof(object).Assembly.Location) + Path.DirectorySeparatorChar;
        CSharpCompilation compilation = CSharpCompilation.Create(
            "assemblyName",
            syntaxTrees,
            new[] { 
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(rtPath + "System.Runtime.dll"),
                MetadataReference.CreateFromFile(rtPath + "System.Collections.dll"),
                MetadataReference.CreateFromFile("geometry4Sharp.dll")
                },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Assembly? assembly = null;
        using (var dllStream = new MemoryStream())
        using (var pdbStream = new MemoryStream())
        {
            var emitResult = compilation.Emit(dllStream, pdbStream);
            if (!emitResult.Success) {
                emitResult.Diagnostics.ToList().ForEach(error => Console.WriteLine(error.ToString()));
                return null;
            }

            assembly = Assembly.Load(((MemoryStream)dllStream).ToArray());

            var module = assembly.GetModules()[0];
            var lrType = module.GetType("LRImplicit");
            return lrType;
        }
    }

    private static string ReadText(string path) {
        string op = "";
        try {
            StreamReader stream = new(path);
            op = stream.ReadToEnd();
            stream.Close();
        }
        catch (Exception e) {
            Console.WriteLine("Exception: " + e.Message);
        }
        return op;
    }
}

