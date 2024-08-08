using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Core.Sprites;

public abstract class BaseSpriteCompInst : BaseCompInst, ISpriteComp
{
    public RotationFlag ViewRotation { get; protected set; }
    public LayerVisibility LayerVisibility { get; protected set; }
    // private List<ISpriteWatcher> _spriteWatchers = new List<ISpriteWatcher>();
    private SpriteData? _currentSprite;
    protected BaseSpriteCompInst(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }
    
    public virtual void DirtySprite()
    {
        Active = true;
        Instance.FlagWatchedChange(SpriteChangeFlags.SpriteDirtied);
    }

    protected abstract SpriteData _UpdateSprite();
    
    public virtual SpriteData GetSprite()
    {
        if(_currentSprite == null)
            Instance.AddChangeWatcher(this.Id, new []{SpriteChangeFlags.SpriteDirtied}, true);
        if (Instance.GetWatchedChange(this.Id, SpriteChangeFlags.SpriteDirtied))
        {
            var sprite = _UpdateSprite();
            _currentSprite = sprite ?? throw new Exception($"Failed to update sprite. {Instance._DebugId}");
            this.Instance.FlagWatchedChange(SpriteChangeFlags.SpriteRefreshed);
        }
        return _currentSprite;
    }
}