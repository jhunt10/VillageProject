using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM.Defs;

public abstract class GenericCompDef<TCompInst, TManager> : BaseCompDef 
    where TCompInst : class, ICompInst
    where TManager : class, IManager
{
    private string _compKey;
    
    [JsonPropertyName("CompDefClassName"), JsonPropertyOrder(-10)]
    public override string CompDefClassName
    {
        get { return this.GetType().FullName; }
    }
    [JsonPropertyName("CompInstClassName"), JsonPropertyOrder(-9)]
    public override string CompInstClassName
    {
        get { return typeof(TCompInst).FullName; }
    }
    [JsonPropertyName("ManagerClassName"), JsonPropertyOrder(-8)]
    public override string ManagerClassName 
    {
        get { return typeof(TManager).FullName; }
    }

    [JsonPropertyName("CompKey"), JsonPropertyOrder(-7)]
    public override string CompKey
    {
        get
        {
            if (string.IsNullOrEmpty(_compKey))
            {
                if (ParentDef == null)
                    _compKey = CompDefClassName;
                else
                {
                    _compKey = ParentDef.DefName + DimMaster.PATH_SEPERATOR 
                            + CompDefClassName.Split('.').Last(); 
                }
            }

            return _compKey;
        }
        set
        {
            _compKey = value;
        }
    }

    internal GenericCompDef<TCompInst, TManager> GetAsBaseClass()
    {
        return this as GenericCompDef<TCompInst, TManager>;
    }

}