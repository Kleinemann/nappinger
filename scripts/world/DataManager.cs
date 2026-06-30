using Godot;
using System.Collections.Generic;

public static class DataManager
{
}

public class V2i
{
    public int X;
    public int Y;

    public V2i() { }

    public V2i(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2I ToVector2I()
    {
        return new Vector2I(X, Y);
    }
}

public class ChunkData
{
    public Vector2I Coords;
    public LayerData[] Layers = new LayerData[5];

    public ChunkData() { }
}

public class LayerData
{
    public int LayerId;
    public List<TileDataCell> TileDataCells = new List<TileDataCell>();

    public LayerData() { }
}

public class TileDataCell
{    
    public V2i Coords;
    public V2i AtlasCoords;
    public int AtlasIndex;

    public TileDataCell() { }
}