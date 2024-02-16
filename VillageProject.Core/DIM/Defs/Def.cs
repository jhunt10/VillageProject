using System.Text.Json.Serialization;

namespace VillageProject.Core.DIM.Defs;

public class Def : IDef
{
    [JsonPropertyName("DefName"), JsonPropertyOrder(0)]
    public string DefName { get; set; }
    
    [JsonPropertyName("Label"), JsonPropertyOrder(1)]
    private string? _label { get; set; }
    public string Label
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_label))
                _label = DefName.Split(DimMaster.PATH_SEPERATOR).Last();
            return _label;
        }

        set
        {
            _label = value;
        }
    }
    
    [JsonIgnore]
    public string LoadPath { get; internal set; }
    
    [JsonPropertyName("CompDefs")]
    public List<ICompDef> CompDefs { get; set; }
}