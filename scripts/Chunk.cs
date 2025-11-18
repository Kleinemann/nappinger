using Godot;
using System;

public partial class Chunk : GodotObject
{
    public static readonly int TileSize = 16;
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkRange = 3;

    public Vector2I Coords;

    public Chunk()
    {
    }

    public Chunk(Vector2I coords)
    {
        Coords = coords;
    }

    public void Paint()
    {
        WorldMain.Instance.Map.WorldLayer.SetCell(Coords * ChunkSize, 0, new Vector2I(5, 3));
    }
}
