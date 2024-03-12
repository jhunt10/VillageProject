using System;
using System.IO;
using Godot;
using Godot.Collections;
using VillageProject.Core.Sprites;
using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;
using VillageProject.Core.Sprites.PatchSprites;
using Array = Godot.Collections.Array;

namespace VillageProject.Godot.Sprites;

public class GodotPatchCellSpriteComp : BasePatchCellSpriteComp, ISpriteComp
{
    public const string SPRITE_KEY = "full_sprite";

    private int _gridWidth;
    private ImageTexture[] _cache; 

    private Image _image;

    public GodotPatchCellSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    protected override SpriteData _UpdateSprite()
    {
        throw new NotImplementedException();
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
            if (_image == null)
                throw new Exception($"Failed to load image from '{path}'.");
        }
        var width = def.SpriteWidth;
        var hight = def.SpriteHight;

        if (_cache == null)
        {
            var imageSize = _image.GetSize();
            _gridWidth = imageSize.X / width;
            var gridHight = imageSize.Y / hight;
            _cache = new ImageTexture[_gridWidth * gridHight];
        }

        var index = (_gridWidth * y) + x;
        if (index > _cache.Length)
            throw new Exception("Atlas Co outside of bounds.");
        if (_cache[index] != null)
            return new SpriteData(_cache[index], new SpriteDataDef("", 32, 32, 0, 0));
        

        var subImage = _image.GetRegion(new Rect2I(x * width, y * hight, width, hight));
        _cache[index] = ImageTexture.CreateFromImage(subImage);
        return new SpriteData(_cache[index], new SpriteDataDef("", 32, 32, 0, 0));
    }
}