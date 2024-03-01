using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Map.Terrain;

namespace VillageProject.Core.Serialization;

public class CompDefJsonConverter : JsonConverter<ICompDef>
{
    public override ICompDef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
             throw new JsonException();
        }

        string? fullText = null;
        using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        {
            fullText = jsonDocument.RootElement.GetRawText();
        }

        var jsonObj = JsonNode.Parse(fullText);
        var className = jsonObj[nameof(ICompDef.CompDefClassName)].ToString();
        var type = GetTypeByName(className);
        if (type == null)
        {
            // throw new Exception($"Failed to find CompDef type with name '{className}'.");
            Console.WriteLine($"Failed to find CompDef type with name '{className}'.");
            return null;
        }

        var deserialize = JsonSerializer.Deserialize(fullText, type);
        var compDef = deserialize as ICompDef;
        if(compDef == null)
            throw new Exception($"Failed to cast type with name '{className}' as ICompDef.");

        return compDef;
    }

    public override void Write(Utf8JsonWriter writer, ICompDef value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, options);
    }

    private Type? GetTypeByName(string name)
    {
        foreach (var assembly in GetAssemblies())// AppDomain.CurrentDomain.GetAssemblies().Reverse())
        {
            var tt = assembly.GetType(name);
            if (tt != null)
            {
                return tt;
            }
        }

        return null;
    }
    
    public static IEnumerable<Assembly> GetAssemblies()
    {
        var list = new List<string>();
        var stack = new Stack<Assembly>();

        stack.Push(Assembly.GetEntryAssembly());

        do
        {
            var asm = stack.Pop();

            yield return asm;

            foreach (var reference in asm.GetReferencedAssemblies())
                if (!list.Contains(reference.FullName))
                {
                    stack.Push(Assembly.Load(reference));
                    list.Add(reference.FullName);
                }

        }
        while (stack.Count > 0);

    }
}