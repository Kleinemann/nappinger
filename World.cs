using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

[Tool]
public partial class World : Node2D
{
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
    int ChunkWidth = 4;

    [Export]
    int ChunkHeigth = 4;

    [Export]
    public bool Generate
    {
        get => false;
        set
        {
            UpdateChunks();
        }
    }


    TileMapLayer _water = null;
    TileMapLayer Water
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

    public override void _Ready()
    {
        UpdateChunks();
    }

    void CleanChunks()
    {
        for (int x = 0; x <= ChunkWidth; x++)
        {
            for (int y = 0; y <= ChunkHeigth; y++)
            {
                if(Ground == null)
                    return;

                Ground.EraseCell(new Vector2I(x, y));
            }
        }
    }

    void RefreshValues()
    {
        if(Ground == null || Water == null)
        {
            GD.PrintErr("TileMapLayer not found, cannot refresh values.");
            return;
        }
        UpdateChunks();
    }

    Vector2I PosToChunk(Vector2 pos)
    {
        return new Vector2I((int)pos.X / ChunkWidth, (int)pos.Y / ChunkHeigth);
    }

    class Tile
    {
        public float Max;
        public float Min;

        public Vector2I[] points = new Vector2I[4];
        public int[] IDs = new int[4];

        public int CountIds
        {
            get
            {

                return IDs.Count(n => n == TerrainFront);
            }
        }


        public Tile(int x, int y)
        {
            points[0] = new Vector2I(x * 2, y * 2);
            points[1] = new Vector2I(x * 2 + 1, y * 2);
            points[2] = new Vector2I(x * 2, y * 2 + 1);
            points[3] = new Vector2I(x * 2 + 1, y * 2 + 1);
        }

        public int TerrainFront
        {
            get
            {
                return ValueToID(Max);
            }
        }

        public int TerrainBack
        {
            get
            {
                return ValueToID(Min);
            }
        }

        public int ValueToID(float value)
        {
            if (value > 0.2)
                return 2; //grass
            else if (value > 0)
                return 1; // sand
            else
                return 0; // water
        }
        

        public Vector2I AtlasPos
        {
            get
            {
                int id = TerrainFront;

                Vector2I offset = new Vector2I(TerrainBack * 4, TerrainFront * 4);

                Vector2I atlas = Vector2I.Zero;
                switch(CountIds)
                {
                    case 1:
                        if (IDs[0] == id)
                            atlas = new Vector2I(3, 3);
                        else if (IDs[1] == id)
                            atlas = new Vector2I(0, 2);
                        else if (IDs[2] == id)
                            atlas = new Vector2I(0, 0);
                        else if (IDs[3] == id)
                            atlas = new Vector2I(1, 3);
                        break;

                    case 2:
                        if (IDs[0] == id)
                        {
                            if (IDs[1] == id)
                                atlas = new Vector2I(1, 2);
                            else if (IDs[2] == id)
                                atlas = new Vector2I(3, 2);
                            else if (IDs[3] == id)
                                atlas = new Vector2I(0, 1);
                        }

                        else if (IDs[2] == id)
                        {
                            if (IDs[1] == id)
                                atlas = new Vector2I(0, 2);
                            else if (IDs[3] == id)
                                atlas = new Vector2I(3, 0);
                        }

                        else if (IDs[1] == id && IDs[3] == id)
                            atlas = new Vector2I(1, 0);

                        break;

                    case 3:
                        if (IDs[0] != id)
                            atlas = new Vector2I(1, 1);
                        else if (IDs[1] != id)
                            atlas = new Vector2I(2, 0);
                        else if (IDs[2] != id)
                            atlas = new Vector2I(1, 1);
                        else if (IDs[3] != id)
                            atlas = new Vector2I(3, 1);
                        break;

                    case 4:
                        atlas = new Vector2I(2, 1);
                        break;
                    default:
                        atlas = new Vector2I(0, 3);
                        break;
                        
                }

                return atlas + offset;
            }
        }
    }


    void UpdateChunks()
    {
        for(int x = 0; x <= ChunkWidth*10;x++)
        {
            for(int y = 0;y <= ChunkHeigth*10;y++)
            {
                Tile t = new Tile(x, y);
                float min = float.MaxValue;
                float max = float.MinValue;
                for(int i  = 0; i < t.points.Length; i++) 
                {
                    Vector2I p = t.points[i];
                    float value = GetNoise(p);
                    t.IDs[i] = t.ValueToID(value);

                    if(value < min) 
                        min = value;

                    if(value > max) 
                        max = value;
                }
                t.Max = max;
                t.Min = min;

                Ground.SetCell(new Vector2I(x, y), 0, t.AtlasPos);
            }
        }

        /*
        Array<Vector2I> waterTiles = new Array<Vector2I>();
        Array<Vector2I> grasTiles = new Array<Vector2I>();
        Array<Vector2I> sandTiles = new Array<Vector2I>();


        for (int x = 0; x <= 32; x++)
        {
            for (int y = 0; y <= 32; y++)
            {
                float z = GetNoise(new Vector2I(x, y));
                Vector2I pos = new Vector2I(x, y);
                Water.SetCell(pos, 0, new Vector2I(2,1)); // Set water tile at position

                if(z > 0)
                {
                    if (z > 0.2)
                    {
                        Ground.SetCell(pos, 1, new Vector2I(2, 5));
                        //grasTiles.Add(pos);
                    }
                    else
                        Ground.SetCell(pos, 2, new Vector2I(2,9));
                    //sandTiles.Add(pos);
                }
            }
        }
        */
        //Ground.SetCellsTerrainConnect(sandTiles, 0, 1);
        //Ground.SetCellsTerrainConnect(grasTiles, 0, 2);

        /*
        Sprite2D sprite = GetNode<Sprite2D>("player");

        Vector2 pos = sprite.GlobalPosition;

        Vector2I chunkPos = PosToChunk(pos);

        GD.Print($"Updating Chunks  around Pos: {pos.ToString()}");

        for(int x = chunkPos.X - ChunkRange; x <= chunkPos.X + ChunkRange; x++)
        {
            for (int y = chunkPos.Y - ChunkRange; y <= chunkPos.Y + ChunkRange; y++)
            {                
                Vector2I offset = new Vector2I(x, y);
                UpdateChunk(chunkPos);
            }
        }
        */
    }


    void UpdateChunk(Vector2I offset)
    {
        /*
        GD.Print($"Updating Chunk offset {offset.ToString()}");

        int waterID = 0;
        int sandID = 1;
        int grassID = 2;

        Array<Vector2I> waterTiles = new Array<Vector2I>();
        Array<Vector2I> grasTiles = new Array<Vector2I>();
        Array<Vector2I> sandTiles = new Array<Vector2I>();

        for (int x = 0; x <= ChunkWidth; x++)
        {
            for (int y = 0; y <= ChunkHeigth; y++)
            {
                Vector2I pos = new Vector2I(x + offset.X, y + offset.Y);
                float value = GetNoise(pos);

                if (value > 0)
                {
                    if (value > 0.2)
                        grasTiles.Add(pos);

                    sandTiles.Add(pos);
                }
                Water.SetCell(pos, waterID, new Vector2I(9, 2));
            }
        }
        */
        /*
        for (int x = -1; x <= ChunkWidth + 1; x++)
        {
            for (int y = -1; y <= ChunkHeigth + 1; y++)
            {
                if (x == -1 || x == ChunkWidth + 1 || y == -1 || y == ChunkHeigth + 1)
                {
                    sandTiles.Add(new Vector2I(x, y));
                }
            }
        }*/

        //Ground.SetCellsTerrainConnect(sandTiles, 0, sandID);
        //Ground.SetCellsTerrainConnect(grasTiles, 0, grassID);
    }



    float GetNoise(Vector2I pos)
    {
        return Noise.GetNoise2D(pos.X, pos.Y);
    }
}
