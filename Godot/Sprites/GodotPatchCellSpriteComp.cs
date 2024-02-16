using Godot;
using VillageProject.Core.Sprites;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;
using VillageProject.Core.Sprites.PatchSprites;

namespace VillageProject.Godot.Sprites;

public class GodotPatchCellSpriteComp : BasePatchCellSpriteComp, ISpriteComp
{
    public const string SPRITE_KEY = "full_sprite";

    private Image _image;

    public GodotPatchCellSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    public override SpriteData GetSprite()
    {
        throw new NotImplementedException();
    }

    protected override SpriteData GetSubSprite(int x, int y)
    {
        var def = GetCompDefAs<IPatchSpriteCompDef>();
        if (_image == null)
        {
            var path = Path.Combine(def.ParentDef.LoadPath, def.SpriteSheet);
            _image = Image.LoadFromFile(path);
        }
        var width = def.SpriteWidth;
        var hight = def.SpriteHight;

        var subImage = _image.GetRegion(new Rect2I(x * width, y * hight, width, hight));
        return new SpriteData(ImageTexture.CreateFromImage(subImage));
    }
}