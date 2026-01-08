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

    public Vector2I[] GetTileCoords()
    {
        Vector2I[] coords = new Vector2I[ChunkSize * ChunkSize];
        Vector2I start = (Coords * ChunkSize);
        Vector2I end = start + new Vector2I(ChunkSize - 1, ChunkSize - 1);

        int index = 0;
        for (var x = start.X; x <= end.X; x++)
        {
            for (var y = start.Y; y <= end.Y; y++)
            {
                Vector2I tileCoord = new Vector2I(x, y);
                coords[index] = tileCoord;
                index++;
            }
        }
        return coords;
    }

    public void Paint()
    {
        foreach (Vector2I tileCoord in GetTileCoords())
        {
            WorldMain.Instance.Map.WorldLayer.SetCell(tileCoord, 0, new Vector2I(5, 3));
        }
    }

    public void Clean()
    {
        foreach(Vector2I tileCoord in GetTileCoords())
        {
            WorldMain.Instance.Map.WorldLayer.EraseCell(tileCoord);
        }
    }
}
