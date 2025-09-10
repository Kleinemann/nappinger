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


    public enum TileType { WATER, SAND, GRASS };

    public override void _Ready()
    {
        OffsetLayer = GetNode<TileMapLayer>("OffsetGrid");
        WorldLayer = GetNode<TileMapLayer>("WorldGrid");
    }

    public void setTile(Vector2I pos, Vector2I coord, TileType tileType)
    {
        
        WorldLayer.SetCell(pos, 0, coord);

        Vector2I newPos = pos + NEIGHBOURS[0];
        GD.Print("-" + newPos);
        RefreshOffset(newPos);
        
        //foreach (Vector2I ce in GetNeigbours(pos))
        //{
        //RefreshOffset(ce);
        //OffsetLayer.SetCell(ce, 0, coord);
        //OffsetLayer.SetCell(ce, 0, new Vector2I(2,1));
        //}
    }

    public void RefreshOffset(Vector2I pos)
    {
        Vector2I v1 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[0]);
        Vector2I v2 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[1]);
        Vector2I v3 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[2]);
        Vector2I v4 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[3]);

        GD.Print("--" + v1);
        GD.Print("--" + v2);
        GD.Print("--" + v3);
        GD.Print("--" + v4);

        int a = values.IndexOf(v1);
        int b = values.IndexOf(v2);
        int c = values.IndexOf(v3);
        int d = values.IndexOf(v4);

        int max = Math.Max(Math.Max(a, b), Math.Max(c, d));
        int min = Math.Min(Math.Min(a, b), Math.Min(c, d));

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

        OffsetLayer.SetCell(pos, 0, neighboursToAtlasCoord[new(a, b, c, d)]);
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
        if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector2I coords = WorldLayer.LocalToMap(mousePos);
            Vector2I atlasCord = Vector2I.Zero;
            TileType type = TileType.WATER;

            GD.Print(coords);

            if(mouseEvent.ButtonIndex == MouseButton.Left)
            {
                atlasCord = new Vector2I(2, 5);
                type = TileType.SAND;
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                atlasCord = new Vector2I(2, 9);
                type = TileType.GRASS;
            }
            else if(mouseEvent.ButtonIndex == MouseButton.Middle)
            {
                atlasCord = new Vector2I(2, 1);
            }

            setTile(coords, atlasCord, type);
        }
    }
}
