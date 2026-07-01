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
    public List<TileDataCell> Map = new List<TileDataCell>();
    public List<TileDataCell> Objects = new List<TileDataCell>();
    public List<BuildingTileData> Buildings = new List<BuildingTileData>();

    public ChunkData() { }
}

public class LayerData
{
    public int LayerId;
    public List<TileDataCell> TileDataCells = new List<TileDataCell>();

    public LayerData() { }
}


public class BuildingTileData
{
    public TileDataCell Floor;
    public TileDataCell Wall;
    public TileDataCell Roof;
}


public class TileDataCell
{    
    public V2i Coords;
    public V2i AtlasCoords;
    public int AtlasIndex;
    public int Atlasalternative;

    public TileDataCell() { }

    public TileDataCell(TileMapLayer tileLayer, Vector2I tileCoord)
    {
        Vector2I atlasCoords = tileLayer.GetCellAtlasCoords(tileCoord);

        Coords = new V2i(tileCoord.X, tileCoord.Y);
        AtlasCoords = new V2i(atlasCoords.X, atlasCoords.Y);
        AtlasIndex = tileLayer.GetCellSourceId(tileCoord);
        Atlasalternative = tileLayer.GetCellAlternativeTile(tileCoord);
    }
}