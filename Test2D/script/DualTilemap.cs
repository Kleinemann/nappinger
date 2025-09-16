using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection.Emit;

public partial class DualTilemap : Node2D
{
    List<Vector2I> values = new List<Vector2I>() { new Vector2I(-1, -1), new Vector2I(2, 1), new Vector2I(2, 5), new Vector2I(2, 9) };
    TileMapLayer OffsetLayer;
    TileMapLayer WorldLayer;
    readonly Vector2I[] NEIGHBOURS = new Vector2I[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };


    public enum TileType { NONE, WATER, SAND, GRASS };

    public override void _Ready()
    {
        OffsetLayer = GetNode<TileMapLayer>("OffsetGrid");
        WorldLayer = GetNode<TileMapLayer>("WorldGrid");
    }

    public void setTile(Vector2I pos, Vector2I coord, TileType tileType)
    {
        WorldLayer.SetCell(pos, 0, coord);
        
        foreach (Vector2I ce in GetNeigbours(pos))
        {
            RefreshOffset(ce);
        }
    }

    public void RefreshOffset(Vector2I pos)
    {
        Vector2I m4 = pos - NEIGHBOURS[0];
        Vector2I m3 = pos - NEIGHBOURS[1];
        Vector2I m2 = pos - NEIGHBOURS[2];
        Vector2I m1 = pos - NEIGHBOURS[3];

        Vector2I v1 = WorldLayer.GetCellAtlasCoords(m1);
        Vector2I v2 = WorldLayer.GetCellAtlasCoords(m2);
        Vector2I v3 = WorldLayer.GetCellAtlasCoords(m3);
        Vector2I v4 = WorldLayer.GetCellAtlasCoords(m4);

        GD.Print("--" + m1 + " => " + v1);
        GD.Print("--" + m2 + " => " + v2);
        GD.Print("--" + m3 + " => " + v3);
        GD.Print("--" + m4 + " => " + v4);


        int a = values.IndexOf(v1);
        int b = values.IndexOf(v2);
        int c = values.IndexOf(v3);
        int d = values.IndexOf(v4);

        int max = Math.Max(Math.Max(a, b), Math.Max(c, d));
        int min = max - 1;

        if (a != max) a = min;
        if (b != max) b = min;
        if (c != max) c = min;
        if (d != max) d = min;

        Vector2I offset = Vector2I.Zero;

        if (max == 2)
            offset += new Vector2I(0, 4);

        if (max == 3)
            offset += new Vector2I(0, 8);


        if(min == 2)
            offset += new Vector2I(4, 0);

        if (min == 3)
            offset += new Vector2I(8, 0);

        Vector2I coord;

        if (min == max)
        {
            coord = new Vector2I(2, 1);
        }
        else
        {
            Dictionary<Tuple<int, int, int, int>, Vector2I> neighboursToAtlasCoord = new()
            {
                {new (max, max, max, max), new Vector2I(2, 1)}, // All corners
                {new (min, min, min, max), new Vector2I(1, 3)}, // Outer bottom-right corner
                {new (min, min, max, min), new Vector2I(0, 0)}, // Outer bottom-left corner
                {new (min, max, min, min), new Vector2I(0, 2)}, // Outer top-right corner
                {new (max, min, min, min), new Vector2I(3, 3)}, // Outer top-left corner
                {new (min, max, min, max), new Vector2I(1, 0)}, // Right edge
                {new (max, min, max, min), new Vector2I(3, 2)}, // Left edge
                {new (min, min, max, max), new Vector2I(3, 0)}, // Bottom edge
                {new (max, max, min, min), new Vector2I(1, 2)}, // Top edge
                {new (min, max, max, max), new Vector2I(1, 1)}, // Inner bottom-right corner
                {new (max, min, max, max), new Vector2I(2, 0)}, // Inner bottom-left corner
                {new (max, max, min, max), new Vector2I(2, 2)}, // Inner top-right corner
                {new (max, max, max, min), new Vector2I(3, 1)}, // Inner top-left corner
                {new (min, max, max, min), new Vector2I(2, 3)}, // Bottom-left top-right corners
                {new (max, min, min, max), new Vector2I(0, 1)}, // Top-left down-right corners
		        {new (min, min, min, min), new Vector2I(0, 3)}, // No corners
            };

           coord = neighboursToAtlasCoord[new(a, b, c, d)];
        }
        GD.Print("--- " + coord);

        OffsetLayer.SetCell(pos, 0, coord + offset);
       
    }

    public TileType GetTileType(Vector2I pos)
    {
        //WorldLayer.g
        return TileType.WATER;
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
            atlasCord = new Vector2I(2, 5);
            type = TileType.SAND;
        }
        else if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            atlasCord = new Vector2I(2, 9);
            type = TileType.GRASS;
        }
        else if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            atlasCord = new Vector2I(2, 1);
        }
        else
        {
            return;
        }

        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2I coords = WorldLayer.LocalToMap(mousePos);

        GD.Print(coords);


        if (WorldLayer.GetCellAtlasCoords(coords) != atlasCord)
            setTile(coords, atlasCord, type);
    }
}
