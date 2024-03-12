using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.Enums;
using VillageProject.Core.Map;
using VillageProject.Core.Map.MapStructures;

namespace VillageProject.Core.Serialization;

public class OccupationDataJsonConverter : JsonConverter<OccupationData>
{
    public override OccupationData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var dict = new Dictionary<MapSpot, List<OccupationFlags>>();
        var currentSpot = new MapSpot();
        var currentArray = new List<OccupationFlags>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                currentSpot = new MapSpot(reader.GetString());
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                currentArray = new List<OccupationFlags>();
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                currentArray.Add((OccupationFlags)reader.GetInt32());
            }

            if (reader.TokenType == JsonTokenType.EndArray)
            {
                dict.Add(currentSpot, currentArray);
            }
            
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new OccupationData(dict);
            }
        }

        return new OccupationData(dict);
        // string? fullText = null;
        // using (var jsonDocument = JsonDocument.ParseValue(ref reader))
        // {
        //     fullText = jsonDocument.RootElement.GetRawText();
        // }
        //
        // var jsonObj = JsonNode.Parse(fullText);
        // var deserialize = JsonSerializer.Deserialize(fullText, type);
        // var compDef = deserialize as ICompDef;
        // if(compDef == null)
        //     throw new Exception($"Failed to cast type with name '{className}' as ICompDef.");
        //
        // return compDef;
    }

    public override void Write(Utf8JsonWriter writer, OccupationData value, JsonSerializerOptions options)
    {
        var internalData = value.OccupationDict;
        JsonSerializer.Serialize(writer, (object)internalData, options);
    }

    private Type? GetTypeByName(string name)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
        {
            var tt = assembly.GetType(name);
            if (tt != null)
            {
                return tt;
            }
        }

        return null;
    }
}