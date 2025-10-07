using Godot;
using System;
using System.Collections.Generic;

public partial class DualTilemap : Node2D
{
    [Export]
    public Button[] Checkboxes;

    [Export]
    public Node2D Player;

    public TileMapLayer OffsetLayer;
    public TileMapLayer WorldLayer;

    readonly List<Vector2I> BaseTiles = new List<Vector2I>() { new Vector2I(-1, -1), new Vector2I(2, 1), new Vector2I(2, 5), new Vector2I(2, 9), new Vector2I(2, 13) };
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


    public enum TileType { NONE, WATER, SAND, GRASS, DIRT};
    public TileType SelectedTileType;


    public void ChangeTileType()
    {
        for (int i = 0; i < Checkboxes.Length; i++)
        {
            if (Checkboxes[i].ButtonPressed)
            {
                SelectedTileType = GetTileType(i);
                return;
            }
        }
    }

    public override void _Ready()
    {
        OffsetLayer = GetNode<TileMapLayer>("OffsetGrid");
        WorldLayer = GetNode<TileMapLayer>("WorldGrid");
        ChangeTileType();
    }

    public void CleanTile(Vector2I pos)
    {
        WorldLayer.EraseCell(pos);
        foreach (Vector2I ce in GetNeigbours(pos))
        {
            OffsetLayer.EraseCell(ce);
        }
    }

    public void setTile(Vector2I pos, TileType tileType)
    {
        Vector2I coord = BaseTiles[(int)tileType];
        WorldLayer.SetCell(pos, 0, coord);

        //fixing Neigbours first
        foreach (Vector2I offset in NEIGHBOURS_AROUND)
        {
            Vector2I nPos = pos + offset;

            int t = (int)tileType;
            int tN = (int)GetTileType(nPos);

            if ((tN + 1) < t)
                setTile(nPos, GetTileType(t - 1));
            if((t + 1) < tN)
                setTile(nPos, GetTileType(t + 1));
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

    public override void _UnhandledInput(InputEvent @event)
    {
        Vector2I atlasCord = Vector2I.Zero;
        TileType type = SelectedTileType;

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector2I pos = WorldLayer.LocalToMap(mousePos);

            //Nur aendern wenn es sich veraendert hat
            if (GetTileType(pos) != type)
                setTile(pos, type);
        }

        if(Input.IsKeyPressed(Key.Up))
        {
            Player.Position += new Vector2(0, -16);
            World.Instance.UpdateMap();
        }
        if (Input.IsKeyPressed(Key.Down))
        {
            Player.Position += new Vector2(0, 16);
            World.Instance.UpdateMap();
        }
        if (Input.IsKeyPressed(Key.Left))
        {
            Player.Position += new Vector2(-16, 0);
            World.Instance.UpdateMap();
        }
        if (Input.IsKeyPressed(Key.Right))
        {
            Player.Position += new Vector2(16, 0);
            World.Instance.UpdateMap();
        }
    }
}
