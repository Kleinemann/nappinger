using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using static WorldMap;

public partial class Chunk : GodotObject
{
    public static readonly int TileSize = 32;
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkRange = 3;

    public Vector2I Coords;
    public int CellIndex;

    public WorldMap Map => WorldMain.Instance.Map;

    readonly Vector2I[] NEIGHBOURS = new Vector2I[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };
    readonly Vector2I[] NEIGHBOURS_AROUND = new Vector2I[] { new(-1, 0), new(1, 0), new(0, -1), new(0, 1), new(-1, -1), new(1, 1), new(1, -1), new(-1, 1) };
    readonly Dictionary<Tuple<int, int, int, int>, Vector2I> NeighboursToAtlasCoord = new()
    {
        {new (1, 1, 1, 1), new Vector2I(2, 1)}, // All corners
        {new (0, 0, 0, 1), new Vector2I(1, 3)}, // Outer bottom-right corner
        {new (0, 0, 1, 0), new Vector2I(0, 0)}, // Outer bottom-left corner
        {new (0, 1, 0, 0), new Vector2I(0, 2)}, // Outer top-right corner
        {new (1, 0, 0, 0), new Vector2I(3, 3)}, // Outer top-left corner
        {new (0, 1, 0, 1), new Vector2I(1, 0)}, // Right edge
        {new (1, 0, 1, 0), new Vector2I(3, 2)}, // Left edge
        {new (0, 0, 1, 1), new Vector2I(3, 0)}, // Bottom edge
        {new (1, 1, 0, 0), new Vector2I(1, 2)}, // Top edge
        {new (0, 1, 1, 1), new Vector2I(1, 1)}, // Inner bottom-right corner
        {new (1, 0, 1, 1), new Vector2I(2, 0)}, // Inner bottom-left corner
        {new (1, 1, 0, 1), new Vector2I(2, 2)}, // Inner top-right corner
        {new (1, 1, 1, 0), new Vector2I(3, 1)}, // Inner top-left corner
        {new (0, 1, 1, 0), new Vector2I(2, 3)}, // Bottom-left top-right corners
        {new (1, 0, 0, 1), new Vector2I(0, 1)}, // Top-left down-right corners
		{new (0, 0, 0, 0), new Vector2I(0, 3)}, // No corners
    };

    public Chunk()
    {
    }

    public Chunk(Vector2I coords)
    {
        Coords = coords;
        if (FileAccess.FileExists($"user://Chunks//chunk_{Coords.X}_{Coords.Y}.dat"))
            LoadChunk();
        else
            Paint();
    }

    public void Process()
    {
 
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

    float GetNoise(Vector2I tileCoord)
    {
        return GetNoise(tileCoord.X, tileCoord.Y);
    }

    float GetNoise(float x, float z)
    {
        float value = WorldMain.Instance.Map.Noise.GetNoise2D(x, z);
        return value;
    }

    public void Paint()
    {
        foreach (Vector2I tileCoord in GetTileCoords())
        {
            float noiseValue = GetNoise(tileCoord);

            if (noiseValue >= 0.4f)
                SetTile(tileCoord, TileType.DIRT);
            else if (noiseValue >= 0.0f)
                SetTile(tileCoord, TileType.GRASS);
            else if (noiseValue >= -0.2f)
                SetTile(tileCoord, TileType.SAND);
            else
                SetTile(tileCoord, TileType.WATER);

            
            int d4 = (int)(noiseValue * 10000 % 10);
            int d5 = (int)(noiseValue * 100000 % 10);

            //Add DropItems
            if (noiseValue >= 0.150 && noiseValue <= 0.155)
            {
                int amount = d4 % 4;
                if (amount == 0) amount = 1;

                int resourceID = d5;

                string resourceName = string.Empty;
                if (resourceID < 4) resourceName = "food";
                else if (resourceID < 7) resourceName = "stone";
                else resourceName = "wood";

                DropItem item = DropItem.CreateDropItem(resourceName, amount);
                item.Position = tileCoord * TileSize;
                WorldMain.Instance.Map.AddChild(item);
            }

            //add Big Tree / Rock
            else if(noiseValue >= 0.170 && noiseValue <= 0.173)
            {
                Map.ObjectLayer.SetCell(tileCoord, 0, Vector2I.Zero, 1);
            }
            else if(noiseValue >= 0.160 && noiseValue <= 0.164)
            {
                Map.ObjectLayer.SetCell(tileCoord, 0, Vector2I.Zero, 2);
            }
            else if (noiseValue >= 0.165 && noiseValue <= 0.169)
            {
                Map.ObjectLayer.SetCell(tileCoord, 0, Vector2I.Zero, 3);
            }
            //Animals
            else if(noiseValue >= 0.120 && noiseValue <= 0.125)
            {
                if(d4 <= 4)
                    Map.ObjectLayer.SetCell(tileCoord, 1, Vector2I.Zero, 1);
                else
                    Map.ObjectLayer.SetCell(tileCoord, 1, Vector2I.Zero, 2);
            }
        }
    }

    public void SetTile(Vector2I pos, TileType tileType)
    {
        Vector2I coord = Map.TileTypeCoord[tileType];
        Map.WorldLayer.SetCell(pos, 0, coord);

        //TODO: Ränder korigieren !

        foreach (Vector2I ce in GetNeigbours(pos))
        {
            RefreshOffset(ce);
        }
    }

    public void RefreshOffset(Vector2I pos)
    {
        Vector2I[] nPos = new Vector2I[4];
        Vector2I[] nCoords = new Vector2I[4];
        int[] nIndex = new int[4];

        int max = 0;

        for (int i = 0; i < 4; i++)
        {
            nPos[i] = pos - NEIGHBOURS[3 - i]; //Desc, because of direction
            nCoords[i] = Map.WorldLayer.GetCellAtlasCoords(nPos[i]);
            nIndex[i] = Map.BaseTiles.IndexOf(nCoords[i]);

            if (nIndex[i] > max)
                max = nIndex[i];
        }

        if(max == 0)
        {
            //delete cell is emtpy
            Map.OffsetLayer.EraseCell(pos);
            return;
        }

        //Calc offset for Atlas Coords
        Vector2I offset = Vector2I.Zero;
        if (max > 1)
            offset.X = (max - 1) * 4;

        // convert for the tuple
        Vector2I coord;
        for (var i = 0; i < 4; i++)
        {
            nIndex[i] = nIndex[i] == max ? 1 : 0;
        }

        // get the atlas coords
        coord = NeighboursToAtlasCoord[new(nIndex[0], nIndex[1], nIndex[2], nIndex[3])];

        Map.OffsetLayer.SetCell(pos, 0, coord + offset);
    }


    public TileType GetTileType(int index)
    {
        Array values = Enum.GetValues(typeof(TileType));
        TileType t = (TileType)values.GetValue(index);
        return t;
    }


    public TileType GetTileType(Vector2I pos)
    {
        int index = Map.BaseTiles.IndexOf(Map.WorldLayer.GetCellAtlasCoords(pos));
        return GetTileType(index);
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

    public void Clean()
    {
        SaveChunk();

        TileMapLayer[] layers = new TileMapLayer[] { Map.WorldLayer, Map.ObjectLayer, Map.BuildingFloor, Map.BuildingWalls, Map.BuildingRoof };

        //Karte Löschen
        foreach (Vector2I tileCoord in GetTileCoords())
        {
            foreach(TileMapLayer layer in layers)
            {
                layer.EraseCell(tileCoord);
            }

            RefreshOffset(tileCoord);
        }

        //Nodes löschen
        List<Node> del = GetNodesInChunk();
        foreach (Node node in del)
        {
            node.QueueFree();
        }
    }


    public void SaveChunk()
    {
         ChunkData chunkData = new ChunkData();

        foreach (Vector2I tileCoord in GetTileCoords())
        {
            //Map
            chunkData.Map.Add(new TileDataCell(Map.WorldLayer, tileCoord));

            //Objects
            TileDataCell objectCell = new TileDataCell(Map.ObjectLayer, tileCoord);
            if(objectCell.AtlasCoords.X > -1)
            {
                chunkData.Objects.Add(objectCell);
            }

            //Buildings
            TileDataCell floorCell = new TileDataCell(Map.ObjectLayer, tileCoord);
            TileDataCell wallCell = new TileDataCell(Map.ObjectLayer, tileCoord);
            TileDataCell roofCell = new TileDataCell(Map.ObjectLayer, tileCoord);
            if(floorCell.AtlasCoords.X > -1)
            {
                BuildingTileData buildingTileData = new BuildingTileData()
                {
                    Floor = floorCell,
                    Wall = wallCell,
                    Roof = roofCell
                };
                chunkData.Buildings.Add(buildingTileData);
            }
        }


        FileAccess file = FileAccess.Open($"user://Chunks//chunk_{Coords.X}_{Coords.Y}.dat", FileAccess.ModeFlags.Write);

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        string json = JsonSerializer.Serialize(chunkData, options);
        file.StoreLine(json);
    }

    public void LoadChunk()
    {
        FileAccess file = FileAccess.Open($"user://Chunks//chunk_{Coords.X}_{Coords.Y}.dat", FileAccess.ModeFlags.Read);

        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        ChunkData chunkData = JsonSerializer.Deserialize<ChunkData>(file.GetAsText(), options);

        foreach(TileDataCell cell in chunkData.Map)
        {
            Map.WorldLayer.SetCell(cell.Coords.ToVector2I(), cell.AtlasIndex, cell.AtlasCoords.ToVector2I(), cell.Atlasalternative);
            RefreshOffset(cell.Coords.ToVector2I());
        }

        foreach (TileDataCell cell in chunkData.Objects)
        {
            Map.ObjectLayer.SetCell(cell.Coords.ToVector2I(), cell.AtlasIndex, cell.AtlasCoords.ToVector2I(), cell.Atlasalternative);
        }

        foreach(BuildingTileData cell in chunkData.Buildings)
        {
            Map.BuildingFloor.SetCell(cell.Floor.Coords.ToVector2I(), cell.Floor.AtlasIndex, cell.Floor.AtlasCoords.ToVector2I(), cell.Floor.Atlasalternative);
            Map.BuildingWalls.SetCell(cell.Wall.Coords.ToVector2I(), cell.Wall.AtlasIndex, cell.Wall.AtlasCoords.ToVector2I(), cell.Wall.Atlasalternative);
            Map.BuildingRoof.SetCell(cell.Roof.Coords.ToVector2I(), cell.Roof.AtlasIndex, cell.Roof.AtlasCoords.ToVector2I(), cell.Roof.Atlasalternative);
        }
    }


    public List<Node> GetNodesInChunk()
    {
        List<Node> nodes = new List<Node>();
        Rect2 region = new Rect2(Coords * ChunkSize * TileSize, new Vector2(ChunkSize * TileSize, ChunkSize * TileSize));

        foreach(Node node in Map.GetChildren())
        {
            if (node is TileMapLayer)
                continue;

            if (node is Area2D || node is Node2D)
            {
                nodes.Add(node);
            }

            if(node is Area2D a2)
            {
                if (region.HasPoint(a2.Position))
                    nodes.Add(node);
            }

            if(node is Node2D n2)
            {
                if(region.HasPoint(n2.Position))
                    nodes.Add(node);
            }
        }

        return nodes;
    }
}
