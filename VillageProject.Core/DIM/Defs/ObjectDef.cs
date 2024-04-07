using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM.Defs;

public class ObjectDef : BaseDef
{
    [JsonPropertyName("DefClassName"), JsonPropertyOrder(-10)]
    public override string DefClassName
    {
        get { return this.GetType().FullName; }
    }
    [JsonPropertyName("InstClassName"), JsonPropertyOrder(-9)]
    public override string InstClassName
    {
        get { return typeof(ObjectInst).FullName; }
    }
    [JsonPropertyName("ManagerClassName"), JsonPropertyOrder(-8)]
    public override string ManagerClassName 
    {
        get { return typeof(DimMaster).FullName; }
    }
}