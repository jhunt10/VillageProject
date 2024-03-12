using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Enums;
using VillageProject.Core.Sprites.Interfaces;

namespace VillageProject.Core.Sprites;

public abstract class BaseSpriteComp : BaseCompInst, ISpriteComp
{
    public RotationFlag ViewRotation { get; protected set; }
    private List<ISpriteWatcher> _spriteWatchers = new List<ISpriteWatcher>();
    private SpriteData? _currentSprite;
    private bool _dirtySprite;
    protected BaseSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        _dirtySprite = true;
    }
    
    public virtual void DirtySprite()
    {
        _dirtySprite = true;
    }

    public virtual void SetViewRotation(RotationFlag viewRotation)
    {
        if (ViewRotation != viewRotation)
        {
            ViewRotation = viewRotation;
            UpdateSprite();
        }
    }

    protected abstract SpriteData _UpdateSprite();
    
    public void UpdateSprite()
    {
        var sprite = _UpdateSprite();
        _dirtySprite = false;
        _currentSprite = sprite;
        NotifyWatchers();
    }

    public virtual SpriteData GetSprite()
    {
        if (_currentSprite == null || _dirtySprite)
            UpdateSprite();
        return _currentSprite;
    }

    public void AddSpriteWatcher(ISpriteWatcher watcher)
    {
        if(!_spriteWatchers.Contains(watcher))
            _spriteWatchers.Add(watcher);
    }

    protected void NotifyWatchers()
    {
        foreach (var watcher in _spriteWatchers)
        {
            watcher.OnSpriteUpdate();
        }
    }

    public override void Update()
    {
        if (_dirtySprite)
        {
            UpdateSprite();
        }
            
    }
}