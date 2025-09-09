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

        foreach (Vector2I ce in GetNeigbours(pos))
        {
            RefreshOffset(ce);
            //OffsetLayer.SetCell(ce, 0, coord);
        }
    }

    public void RefreshOffset(Vector2I pos) 
    {
        Vector2I v1 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[0]);
        Vector2I v2 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[1]);
        Vector2I v3 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[2]);
        Vector2I v4 = WorldLayer.GetCellAtlasCoords(pos + NEIGHBOURS[3]);

        int a = values.IndexOf(v1);
        int b = values.IndexOf(v2);
        int c = values.IndexOf(v3);
        int d = values.IndexOf(v4);

        //OffsetLayer.SetCell(ce, 0, coord);
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
