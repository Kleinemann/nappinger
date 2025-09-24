using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection.Emit;
using System.Threading;

public partial class DualTilemap : Node2D
{
    TileMapLayer OffsetLayer;
    TileMapLayer WorldLayer;

    readonly List<Vector2I> BaseTiles = new List<Vector2I>() { new Vector2I(-1, -1), new Vector2I(2, 1), new Vector2I(2, 5), new Vector2I(2, 9) };
    readonly Vector2I[] NEIGHBOURS = new Vector2I[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };
    readonly Vector2I[] NEIGHBOURS_AROUND = new Vector2I[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, -1), new(1, 1), new(1, -1), new(-1, 1) };
    readonly Dictionary<Tuple<int, int, int, int>, Vector2I> NeighboursToAtlasCoord = new()
    {
        {new (1, 1, 1, 1), new Vector2I(2, 1)}, // All corners
        {new (0, 0, 0, 1), new Vector2I(1, 3)}, // Outer bottom-right corner
        {new (0, 0, 1, 0), new Vector2I(0, 0)}, // Outer bottom-left corner
        {new (0, 1, 0, 0), new Vector2I(0, 2)}, // Outer top-right corner
        {new (1, 0, 0, 0), new Vector2I(3, 3)}, // Outer top-left corner
        {new (0, 1, 0, 1), new Vector2I(1, 0)}, // Right edge
        {new (1, 0, 1, 0), new Vector2I(3, 2)}, // Left edge
        {new (0, 0, 1, 1), new Vector2I(3, 0)}, // Bottom edge
        {new (1, 1, 0, 0), new Vector2I(1, 2)}, // Top edge
        {new (0, 1, 1, 1), new Vector2I(1, 1)}, // Inner bottom-right corner
        {new (1, 0, 1, 1), new Vector2I(2, 0)}, // Inner bottom-left corner
        {new (1, 1, 0, 1), new Vector2I(2, 2)}, // Inner top-right corner
        {new (1, 1, 1, 0), new Vector2I(3, 1)}, // Inner top-left corner
        {new (0, 1, 1, 0), new Vector2I(2, 3)}, // Bottom-left top-right corners
        {new (1, 0, 0, 1), new Vector2I(0, 1)}, // Top-left down-right corners
		{new (0, 0, 0, 0), new Vector2I(0, 3)}, // No corners
    };


    public enum TileType { NONE, WATER, SAND, GRASS };

    public override void _Ready()
    {
        OffsetLayer = GetNode<TileMapLayer>("OffsetGrid");
        WorldLayer = GetNode<TileMapLayer>("WorldGrid");
    }

    public void setTile(Vector2I pos, TileType tileType)
    {
        //Vector2I v = WorldLayer.GetCellAtlasCoords(pos);
        //TileType tt = GetTileType(v);
        //GD.Print(v + " => " + tt);

        Vector2I coord = BaseTiles[(int)tileType];
        WorldLayer.SetCell(pos, 0, coord);

        //fixing Neigbours first
        foreach (Vector2I offset in NEIGHBOURS_AROUND)
        {
            Vector2I nPos = pos + offset;

            //GD.Print(pos + " => " + nPos);
            int t = (int)tileType;
            int tN = (int)GetTileType(nPos);

            if ((tN + 1) < t)
            {
                //GD.Print(t + " => " + tN);
                //Vector2I newCoord = (BaseTiles[t - 1]);
                //WorldLayer.SetCell(nPos, 0, newCoord);
                setTile(nPos, GetTileType(t - 1));
            }
        }

        foreach (Vector2I ce in GetNeigbours(pos))
        {            
            RefreshOffset(ce);
        }
    }

    public void RefreshOffset(Vector2I pos)
    {
        Vector2I[] nPos = new Vector2I[4];
        Vector2I[] nCoords = new Vector2I[4];
        int[] nIndex = new int[4];

        int max = 0;

        for (int i = 0; i < 4; i++)
        {
            nPos[i] = pos - NEIGHBOURS[3 - i]; //Desc, bacause of direction
            nCoords[i] = WorldLayer.GetCellAtlasCoords(nPos[i]);
            nIndex[i] = BaseTiles.IndexOf(nCoords[i]);

            if(nIndex[i] > max)
                max = nIndex[i];
        }

        int min = max - 1;

        //Calc offset for Atlas Coords
        Vector2I offset = Vector2I.Zero;
        if (max > 1)
            offset.Y = (max - 1) * 4;

        if (min > 1)
            offset.X = (min - 1) * 4;


        // convert for the tuple
        Vector2I coord;
        for (var i = 0; i < 4; i++)
        {
            nIndex[i] = nIndex[i] == max ? 1 : 0;
        }

        // get the atlas coords
        if (min == max)
        {
            coord = new Vector2I(2, 1);
        }
        else
        {
            coord = NeighboursToAtlasCoord[new(nIndex[0], nIndex[1], nIndex[2], nIndex[3])];
        }

        OffsetLayer.SetCell(pos, 0, coord + offset);
       
    }

    public TileType GetTileType(int index)
    {
        Array values = Enum.GetValues(typeof(TileType));
        TileType t = (TileType)values.GetValue(index);
        return t;
    }


    public TileType GetTileType(Vector2I pos)
    {
        int index = BaseTiles.IndexOf(WorldLayer.GetCellAtlasCoords(pos));
        return GetTileType(index);
    }

    public Vector2I[] GetNeigbours(Vector2I pos)
    {
        Vector2I[] posN = new Vector2I[4];
        var index = 0;

        foreach (Vector2I n in NEIGHBOURS)
        {
            posN[index++] = pos + n;
        }
        return posN;
    }

    public override void _Input(InputEvent @event)
    {
        Vector2I atlasCord = Vector2I.Zero;
        TileType type = TileType.WATER;

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            type = TileType.SAND;
        }
        else if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            type = TileType.GRASS;
        }
        else if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            type = TileType.WATER;
        }
        else
        {
            return;
        }

        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2I pos = WorldLayer.LocalToMap(mousePos);

        //Nur aendern wenn es sich veraendert hat
        if(GetTileType(pos) != type)
            setTile(pos, type);
    }
}
