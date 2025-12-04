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
        int halfchunsize = ChunkSize / 2;
        Vector2I start = Coords * ChunkSize - new Vector2I(halfchunsize, halfchunsize);
        Vector2I end = Coords * ChunkSize + new Vector2I(halfchunsize-1, halfchunsize-1);

        //WorldMain.Instance.Map.WorldLayer.SetCell(Coords * halfchunsize, 0, new Vector2I(5, 3));

        for(var x = start.X; x <= end.X; x++)
        {
            WorldMain.Instance.Map.WorldLayer.SetCell(new Vector2I(x, start.Y), 0, new Vector2I(5, 3));
            WorldMain.Instance.Map.WorldLayer.SetCell(new Vector2I(x, end.Y), 0, new Vector2I(5, 3));
        }

        for(var y = start.Y; y <= end.Y; y++)
        {
            WorldMain.Instance.Map.WorldLayer.SetCell(new Vector2I(start.X, y), 0, new Vector2I(5, 3));
            WorldMain.Instance.Map.WorldLayer.SetCell(new Vector2I(end.X, y), 0, new Vector2I(5, 3));
        }
        //WorldMain.Instance.Map.WorldLayer.SetCell(start, 0, new Vector2I(5, 3));
        //WorldMain.Instance.Map.WorldLayer.SetCell(end, 0, new Vector2I(5, 3));
    }
}
