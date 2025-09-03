using Godot;
using System;
using System.ComponentModel.Design;
using System.Reflection.Emit;

public partial class DualTilemap : Node2D
{
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
        foreach (Vector2I ce in GetNeigbours(pos))
        {
            OffsetLayer.SetCell(ce, 0, coord);
        }
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
                type = TileType.WATER;
            }
            else if(mouseEvent.ButtonIndex == MouseButton.Middle)
            {
                atlasCord = new Vector2I(2, 1);
            }

            WorldLayer.SetMeta("Tile", (int)type);
            setTile(coords, atlasCord, type);
        }
    }
}
