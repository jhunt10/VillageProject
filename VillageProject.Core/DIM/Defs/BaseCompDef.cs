using System.Text.Json.Serialization;
using VillageProject.Core.DIM.Insts;

namespace VillageProject.Core.DIM.Defs;

public abstract class BaseCompDef<TCompInst, TManager> : RootCompDef 
    where TCompInst : class, ICompInst
    where TManager : class, IManager
{
    private string _compKey;
    
    [JsonPropertyName("CompDefClassName"), JsonPropertyOrder(0)]
    public override string CompDefClassName
    {
        get { return this.GetType().FullName; }
    }
    public override string CompInstClassName
    {
        get { return typeof(TCompInst).FullName; }
    }
    public override string ManagerClassName 
    {
        get { return typeof(TManager).FullName; }
    }

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

    internal BaseCompDef<TCompInst, TManager> GetAsBaseClass()
    {
        return this as BaseCompDef<TCompInst, TManager>;
    }

}