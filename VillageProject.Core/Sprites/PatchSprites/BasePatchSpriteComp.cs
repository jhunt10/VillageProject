﻿using VillageProject.Core.DIM.Defs;
using VillageProject.Core.DIM.Insts;
using VillageProject.Core.Map;

namespace VillageProject.Core.Sprites.PatchSprites;

public abstract class BasePatchCellSpriteComp : BaseSpriteComp, ISpriteComp
{
    public const string SPRITE_KEY = "full_sprite";
    
    public BasePatchCellSpriteComp(ICompDef def, IInst inst) : base(def, inst)
    {
        
    }

    protected abstract SpriteData GetSubSprite(int x, int y);

    public SpriteData GetPatchSprite(Func<AdjacencyFlags> getAdj)
    {
        var adj = getAdj();
        var atlasCo = AdjacencyToAtlasCo(adj);
        return GetSubSprite(atlasCo[0], atlasCo[1]);
    }
    
    /// <summary>
    /// Returns Atlas Coordiants of tile sprite based on adjacent tiles.
    /// Always matches on side. Only matches on corners if sides are present.
    /// If there is no Left or Back, we don't care what BackLeft is
    /// See VillageProject.Core.Sprites.TileSpriteTemplate.png for example
    /// </summary>
    public static int[] AdjacencyToAtlasCo(AdjacencyFlags adj)
    {
        if(((int)adj) < 0)
            return new int[] { 10, 1 };
        // Has Back
        if (adj.HasFlag(AdjacencyFlags.Back))
        {
            if (adj.HasFlag(AdjacencyFlags.Left))
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, BackRight, FrontLeft, FrontRight
                                        // Has All
                                        return new int[] { 9, 2 };
                                    }
                                    // No Front Right
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, BackRight, FrontLeft
                                        return new int[] { 6, 2 };
                                    }
                                }
                                // No Front Left
                                else
                                {
                                    // Has Back, Left, Right, Front, BackLeft, BackRight, FrontRight
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        return new int[] { 5, 2 };
                                    }
                                    // Has Back, Left, Right, Front, BackLeft, BackRight
                                    else
                                    {
                                        return new int[] { 10, 3 };
                                    }
                                }
                            }
                            // No BackRight
                            else
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, FrontLeft, FrontRight
                                        return new int[] { 6, 1 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, FrontLeft
                                        return new int[] { 11, 1 };
                                    }
                                }
                                // No FrontLeft
                                else
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackLeft, FrontRight
                                        return new int[] { 10, 2 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackLeft
                                        return new int[] { 4, 0 };
                                    }
                                }
                            }
                        }
                        // No BackLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, BackRight, FrontLeft, FrontRight
                                        // Has All
                                        return new int[] { 5, 1 };
                                    }
                                    // No Front Right
                                    else
                                    {
                                        // Has Back, Left, Right, Front, BackRight, FrontLeft
                                        return new int[] { 9, 1 };
                                    }
                                }
                                // No Front Left
                                else
                                {
                                    // Has Back, Left, Right, Front, BackRight, FrontRight
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        return new int[] { 8, 2 };
                                    }
                                    // Has Back, Left, Right, Front, BackRight
                                    else
                                    {
                                        return new int[] { 7, 0 };
                                    }
                                }
                            }
                            // No BackRight
                            else
                            {
                                if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, FrontLeft, FrontRight
                                        return new int[] { 9, 0 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front, FrontLeft
                                        return new int[] { 4, 3 };
                                    }
                                }
                                // No FrontLeft
                                else
                                {
                                    if (adj.HasFlag(AdjacencyFlags.FrontRight))
                                    {
                                        // Has Back, Left, Right, Front, FrontRight
                                        return new int[] { 7, 3 };
                                    }
                                    // No FrontRight
                                    else
                                    {
                                        // Has Back, Left, Right, Front
                                        return new int[] { 2, 1 };
                                    }
                                }
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                // Has Back, Left, Right, BackLeft, BackRight
                                return new int[] { 9, 3 };
                            }
                            // No BackRight
                            else
                            {
                                // Has Back, Left, Right, BackLeft
                                return new int[] { 6, 3 };
                            }
                        }
                        // No BackLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.BackRight))
                            {
                                // Has Back, Left, Right, BackRight
                                return new int[] { 5, 3 };
                            }
                            // No BackRight
                            else
                            {
                                // Has Back, Left, Right
                                return new int[] { 2, 2 };
                            }
                        }
                    }
                }
                // No Right
                else
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                            {
                                // Has Back, Left, Front, BackLeft, FrontLeft
                                return new int[] { 11, 2 };
                            }
                            // No Front Left
                            else
                            {
                                // Has Back, Left, Front, BackLeft
                                return new int[] { 7, 2 };
                            }
                        }
                        // No BackLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                            {
                                // Has Back, Left, Front, FrontLeft
                                return new int[] { 11, 2 };
                            }
                            // No Front Left
                            else
                            {
                                // Has Back, Left, Front
                                return new int[] { 3, 1 };
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackLeft))
                        {
                            // Has Back, Left, BackLeft
                            return new int[] { 11, 3 };
                        }
                        // No BackLeft
                        else
                        {
                            // Has Back, Left,
                            return new int[] { 3, 2 };
                        }
                    }
                }
            }
            // No Left
            else
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackRight))
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Back, Right, Front, BackRight, FrontRight
                                return new int[] { 8, 1 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Back, Right, Front, BackRight
                                return new int[] { 4, 2 };
                            }
                        }
                        // No BackRight
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Back, Right, Front, FrontRight
                                return new int[] { 4, 1 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Back, Right, Front
                                return new int[] { 1, 1 };
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        if (adj.HasFlag(AdjacencyFlags.BackRight))
                        {
                            // Has Back, Right, BackRight
                            return new int[] { 8, 3 };
                        }
                        // No BackRight
                        else
                        {
                            // Has Back, Right
                            return new int[] { 1, 2 };
                        }
                    }
                }
                // No Right
                else
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        // Has Back, Front
                        return new int[] { 0, 1 };

                    }
                    // No Front
                    else
                    {
                        // Has Back
                        return new int[] { 0, 2 };
                    }
                }
            }
        }
        // No Back
        else
        {
            if (adj.HasFlag(AdjacencyFlags.Left))
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Left, Right, Front, FrontLeft, FrontRight
                                return new int[] { 10, 0 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Left, Right, Front, FrontLeft
                                return new int[] { 6, 0 };
                            }
                        }
                        // No FrontLeft
                        else
                        {
                            if (adj.HasFlag(AdjacencyFlags.FrontRight))
                            {
                                // Has Left, Right, Front, FrontRight
                                return new int[] { 5, 0 };
                            }
                            // No FrontRight
                            else
                            {
                                // Has Left, Right, Front
                                return new int[] { 2, 0 };
                            }
                        }
                    }
                    // No Front
                    else
                    {
                        // Has Left Right
                        return new int[] { 2, 3 };
                    }
                }

                // No Right
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.FrontLeft))
                        {
                            // Has Left, Front, FrontLeft
                            return new int[] { 11, 0 };
                        }
                        // No Front Left
                        else
                        {
                            // Has Left, Front
                            return new int[] { 3, 0 };
                        }

                    }
                    // No Front
                    else
                    {
                        // Has Left
                        return new int[] { 3, 3 };
                    }
                }
            }
            // No Left
            else
            {
                if (adj.HasFlag(AdjacencyFlags.Right))
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        if (adj.HasFlag(AdjacencyFlags.FrontRight))
                        {
                            // Has Right, Front, FrontRight
                            return new int[] { 8, 0 };
                        }
                        // No FrontRight
                        else
                        {
                            // Has Right Front
                            return new int[] { 1, 0 };
                        }
                    }
                    // No Front
                    else
                    {
                        // Has Right
                        return new int[] { 1, 3 };

                    }
                }
                // No Right
                else
                {
                    if (adj.HasFlag(AdjacencyFlags.Front))
                    {
                        // Has Front
                        return new int[] { 0, 0 };
                    }
                    // No FrontRight
                    else
                    {
                        // Has None
                        return new int[] { 0, 3 };
                    }
                }
            }
        }
    }
}