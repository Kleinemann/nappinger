using Godot;
using System.Collections.Generic;

public static class DataManager
{
}


public class PlayerData
{
    public PlayerData() { }

    public List<PlayerAnimalData> Players = new List<PlayerAnimalData>();
    public List<StoreData> Stores = new List<StoreData>();
}


public class ChunkData
{
    public Vector2I Coords;
    public List<TileDataCell> Map = new List<TileDataCell>();
    public List<TileDataCell> Objects = new List<TileDataCell>();
    public List<BuildingTileData> Buildings = new List<BuildingTileData>();

    public List<DropItemData> DropItems = new List<DropItemData>();

    public ChunkData() { }
}

public class PlayerAnimalData
{
    public string ObjectName;
    public Vector2 Position;
    public string IconPath;
    public float Speed;
    public int Healt;
    public int MaxHealt;
    public GameObjectState State;
    public string MissionString;

    public List<InvetoryItemData> Inventory = new List<InvetoryItemData>();

    public PlayerAnimalData() { }
}

public class StoreData
{
    public Vector2I Coords;
    public string SceneFilePath;
    public List<InvetoryItemData> Inventory = new List<InvetoryItemData>();
}


public class InvetoryItemData
{
    public string ResourceName;
    public int Amount;
}


public class DropItemData
{
    public Vector2I Coords;
    public string ResourceName;
    public int Amount;
}


public class BuildingTileData
{
    public TileDataCell Floor;
    public TileDataCell Wall;
    public TileDataCell Roof;
}


public class TileDataCell
{    
    public Vector2I Coords;
    public Vector2I AtlasCoords;
    public int AtlasIndex;
    public int Atlasalternative;

    public TileDataCell() { }

    public TileDataCell(TileMapLayer tileLayer, Vector2I tileCoord)
    {
        Vector2I atlasCoords = tileLayer.GetCellAtlasCoords(tileCoord);

        Coords = tileCoord;
        AtlasCoords = atlasCoords;
        AtlasIndex = tileLayer.GetCellSourceId(tileCoord);
        Atlasalternative = tileLayer.GetCellAlternativeTile(tileCoord);
    }
}