using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using VillageProject.Core.Map;

namespace VillageProject.Core.Serialization;

public class MapSpotJsonConverter : JsonConverter<MapSpot>
{
    public override MapSpot Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        return new MapSpot();

    }

    public override void Write(Utf8JsonWriter writer, MapSpot value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.ToString(), options);
    }

    public override MapSpot ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? strVal = reader.GetString();
        if (string.IsNullOrEmpty(strVal))
            throw new JsonException();
        return new MapSpot(strVal);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, MapSpot value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString());
    }
}