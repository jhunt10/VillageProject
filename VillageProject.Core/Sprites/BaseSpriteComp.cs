using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Core.Sprites;

public abstract class BaseSpriteComp : BaseCompInst, ISpriteComp
{
    public RotationFlag ViewRotation { get; protected set; }
    // private List<ISpriteWatcher> _spriteWatchers = new List<ISpriteWatcher>();
    private SpriteData? _currentSprite;
    private bool _dirtySprite;

    public bool IsDirty => _dirtySprite;
    protected BaseSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        _dirtySprite = true;
    }
    
    public virtual void DirtySprite()
    {
        _dirtySprite = true;
        Instance.FlagWatchedChange(this);
    }

    public virtual void SetViewRotation(RotationFlag viewRotation)
    {
        if (ViewRotation != viewRotation)
        {
            ViewRotation = viewRotation;
            DirtySprite();
        }
    }

    protected abstract SpriteData _UpdateSprite();
    
    public virtual SpriteData GetSprite()
    {
        if (_currentSprite == null || _dirtySprite)
        {
            var sprite = _UpdateSprite();
            _currentSprite = sprite ?? throw new Exception($"Failed to update sprite. {Instance._DebugId}");
            _dirtySprite = false;
        }
        return _currentSprite;
    }
}