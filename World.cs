using Godot;
using System;
using System.Collections.Generic;
using static TileType;

public enum TileType
{
    Water,
    Grass,
    Sand
}

[Tool]
public partial class World : Node2D
{
    #region Variables


    readonly Dictionary<Tuple<TileType, TileType, TileType, TileType>, Vector2I> neighboursToAtlasCoord = new() {
        {new (Grass, Grass, Grass, Grass), new Vector2I(2, 1)}, // All corners
        {new (Sand, Sand, Sand, Grass), new Vector2I(1, 3)}, // Outer bottom-right corner
        {new (Sand, Sand, Grass, Sand), new Vector2I(0, 0)}, // Outer bottom-left corner
        {new (Sand, Grass, Sand, Sand), new Vector2I(0, 2)}, // Outer top-right corner
        {new (Grass, Sand, Sand, Sand), new Vector2I(3, 3)}, // Outer top-left corner
        {new (Sand, Grass, Sand, Grass), new Vector2I(1, 0)}, // Right edge
        {new (Grass, Sand, Grass, Sand), new Vector2I(3, 2)}, // Left edge
        {new (Sand, Sand, Grass, Grass), new Vector2I(3, 0)}, // Bottom edge
        {new (Grass, Grass, Sand, Sand), new Vector2I(1, 2)}, // Top edge
        {new (Sand, Grass, Grass, Grass), new Vector2I(1, 1)}, // Inner bottom-right corner
        {new (Grass, Sand, Grass, Grass), new Vector2I(2, 0)}, // Inner bottom-left corner
        {new (Grass, Grass, Sand, Grass), new Vector2I(2, 2)}, // Inner top-right corner
        {new (Grass, Grass, Grass, Sand), new Vector2I(3, 1)}, // Inner top-left corner
        {new (Sand, Grass, Grass, Sand), new Vector2I(2, 3)}, // Bottom-left top-right corners
        {new (Grass, Sand, Sand, Grass), new Vector2I(0, 1)}, // Top-left down-right corners
		{new (Sand, Sand, Sand, Sand), new Vector2I(0, 3)}, // No corners

        //{new (Sand, Sand, Sand, Sand), new Vector2I(2, 1)}, // All corners
        {new (Water, Water, Water, Sand), new Vector2I(1, 3)}, // Outer bottom-right corner
        {new (Water, Water, Sand, Water), new Vector2I(0, 0)}, // Outer bottom-left corner
        {new (Water, Sand, Water, Water), new Vector2I(0, 2)}, // Outer top-right corner
        {new (Sand, Water, Water, Water), new Vector2I(3, 3)}, // Outer top-left corner
        {new (Water, Sand, Water, Sand), new Vector2I(1, 0)}, // Right edge
        {new (Sand, Water, Sand, Water), new Vector2I(3, 2)}, // Left edge
        {new (Water, Water, Sand, Sand), new Vector2I(3, 0)}, // Bottom edge
        {new (Sand, Sand, Water, Water), new Vector2I(1, 2)}, // Top edge
        {new (Water, Sand, Sand, Sand), new Vector2I(1, 1)}, // Inner bottom-right corner
        {new (Sand, Water, Sand, Sand), new Vector2I(2, 0)}, // Inner bottom-left corner
        {new (Sand, Sand, Water, Sand), new Vector2I(2, 2)}, // Inner top-right corner
        {new (Sand, Sand, Sand, Water), new Vector2I(3, 1)}, // Inner top-left corner
        {new (Water, Sand, Sand, Water), new Vector2I(2, 3)}, // Bottom-left top-right corners
        {new (Sand, Water, Water, Sand), new Vector2I(0, 1)}, // Top-left down-right corners
		{new (Water, Water, Water, Water), new Vector2I(0, 3)}, // No corners
    };



    [Export]
    public int Seed
    {
        set
        {
            Noise.Seed = value;
        }
        get
        {
            return Noise.Seed;
        }
    }

    [Export]
    public FastNoiseLite Noise = new FastNoiseLite();

    [Export]
    int ChunkRange = 0;

    [Export]
    int ChunkWidth = 16;

    [Export]
    int ChunkHeigth = 16;

    [Export]
    public bool Generate
    {
        get => false;
        set
        {
            RefreshValues();
        }
    }


    TileMapLayer _water = null;
    TileMapLayer WaterLayer
    {
        get
        {
            if (_water == null)
                _water = GetNode<TileMapLayer>("tilemap/water");

            return _water;
        }
    }

    TileMapLayer _ground = null;
    TileMapLayer Ground
    {
        get
        {
            if(_ground == null)
                _ground = GetNode<TileMapLayer>("tilemap/ground");

            return _ground;
        }
    }

    TileMapLayer _groundDisplay = null;
    TileMapLayer GroundDisplay
    {
        get
        {
            if (_groundDisplay == null)
                _groundDisplay = GetNode<TileMapLayer>("tilemap/groundDisplay");

            return _groundDisplay;
        }
    }


    readonly Vector2I[] NEIGHBOURS = new Vector2I[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) };


    float MAX = float.MinValue; 


    #endregion

    #region Controlls
    public override void _Ready()
    {
        RefreshValues();
    }

    void RefreshValues()
    {
        if(Noise == null || Ground == null || WaterLayer == null)
        {
            GD.PrintErr("TileMapLayer not found, cannot refresh values.");
            return;
        }
        UpdateChunks();
    }

    #endregion

    #region Chunks

    void CleanChunks()
    {
        for (int x = -1; x <= ChunkWidth+1; x++)
        {
            for (int y = -1; y <= ChunkHeigth+1; y++)
            {
                WaterLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(2, 1));
                GroundDisplay.EraseCell(new Vector2I(x, y));
            }
        }
    }


    void UpdateChunks()
    {
        CleanChunks();

        UpdateChunk(new Vector2I(0, 0));
    }


    void UpdateChunk(Vector2I offset)
    {
        for (int x = 0; x <= ChunkWidth; x++)
        {
            for (int y = 0; y <= ChunkHeigth; y++)
            {
                SetDisplayTile(new Vector2I(x, y));
            }
        }
    }

    void SetDisplayTile(Vector2I pos)
    {
        for(int i =0; i < NEIGHBOURS.Length; i++)
        {
            Vector2I newPos = pos + NEIGHBOURS[i];
            Vector2I atlasPos = CalculateDisplayTile(newPos);

            if (atlasPos == new Vector2I(-1, -1))
                return;

            TileType tileType = GetWorldTile(pos);
            if (tileType == TileType.Grass)
                atlasPos.Y += 8; // Adjust for grass tiles in the atlas

            if (tileType == TileType.Sand)
                atlasPos.Y += 4; // Adjust for dirt tiles in the atlas

            GroundDisplay.SetCell(newPos, 0, atlasPos);
        }
    }

    Vector2I CalculateDisplayTile(Vector2I coords)
    {
        TileType botRight = GetWorldTile(coords - NEIGHBOURS[0]);
        TileType botLeft = GetWorldTile(coords - NEIGHBOURS[1]);
        TileType topRight = GetWorldTile(coords - NEIGHBOURS[2]);
        TileType topLeft = GetWorldTile(coords - NEIGHBOURS[3]);

        Tuple<TileType, TileType, TileType, TileType> key = new(topLeft, topRight, botLeft, botRight);
        if(!neighboursToAtlasCoord.ContainsKey(key))
        {
            return new Vector2I(-1, -1); // Default tile if not found
        }
        var n = neighboursToAtlasCoord[key];
        //GD.Print(n);
        return n;
    }


    TileType GetWorldTile(Vector2I pos)
    {
        float value = GetNoise(pos);

        if (value > 0.2)
            return TileType.Grass;

        if (value > 0.0)
            return TileType.Sand;

        return TileType.Water;
    }

    Vector2I PosToChunk(Vector2 pos)
    {
        return new Vector2I((int)pos.X / ChunkWidth, (int)pos.Y / ChunkHeigth);
    }

    float GetNoise(Vector2I pos)
    {
        return Noise.GetNoise2D(pos.X, pos.Y);
    }

    #endregion
}
