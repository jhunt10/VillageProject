using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public abstract class BaseDef : IDef
{
    [JsonPropertyName("DEF_NAME"), JsonPropertyOrder(0)]
    public string Name { get; set; }
    
    [JsonPropertyName("Label")]
    public string? _label { get; set; }
    public string Label
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_label))
                _label = Name.Split(DefMaster.PATH_SEPERATOR).Last();
            return _label;
        }
    }

    public IDef ParentDef { get; }
    public List<IDef> SubDefs { get; }
}