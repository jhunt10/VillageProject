using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM.Defs;

public abstract class GenericDef<TInst, TManager> : BaseDef 
    where TInst : class, IInst
    where TManager : class, IManager
{
    [JsonPropertyName("DefClassName"), JsonPropertyOrder(-10)]
    public override string DefClassName
    {
        get { return this.GetType().FullName; }
    }
    [JsonPropertyName("InstClassName"), JsonPropertyOrder(-9)]
    public override string InstClassName
    {
        get { return typeof(TInst).FullName; }
    }
    [JsonPropertyName("ManagerClassName"), JsonPropertyOrder(-8)]
    public override string ManagerClassName 
    {
        get { return typeof(TManager).FullName; }
    }

}