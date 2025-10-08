using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node2D
{
    [Export]
    public FastNoiseLite Noise;

    [Export]
    public int ChunkSize = 16;
    [Export]
    public int ChunkRange = 3;

    List<Vector2I> Chunks = new List<Vector2I>();

    public static World Instance;

    DualTilemap map;
    public TileMapLayer plants;
    public override void _Ready()
    {
        Instance = this;
        map = GetNode<DualTilemap>("Tilemap");
        plants = GetNode<TileMapLayer>("Plants");
        UpdateMap();
    }


    Vector2I PosToChunk(Vector2I pos)
    {
        return new Vector2I(pos.X / ChunkSize, pos.Y / ChunkSize);
    }

    public void UpdateMap()
    {
        Vector2I posScreen = (Vector2I)map.Player.Position;
        Vector2I pos = map.WorldLayer.LocalToMap(posScreen);
        Vector2I chunk = PosToChunk(pos);

        List<Vector2I> chunksTmp = new List<Vector2I>();

        for (int x = chunk.X - ChunkRange; x <= chunk.X + ChunkRange; x++)
        {
            for(int y = chunk.Y - ChunkRange; y <= chunk.Y + ChunkRange; y++)
            {
                Vector2I coord = new Vector2I(x, y);

                chunksTmp.Add(coord);
                if (!Chunks.Contains(coord))
                {
                    PaintChunk(coord);
                }
                else
                {
                    Chunks.Remove(coord);
                }
            }
        }

        foreach(Vector2I c in Chunks)
        {
            PaintChunk(c, true);
        }

        Chunks = chunksTmp;
    }

    public void PaintChunk(Vector2I coord, bool clean = false)
    {
        float max = float.MinValue;
        float min = float.MaxValue;

        Vector2I Pos = coord * ChunkSize;

        for (int x = 0; x < ChunkSize; x++)
        {
            for(int y = 0; y < ChunkSize; y++)
            {
                Vector2I newPos = new Vector2I(Pos.X + x, Pos.Y + y);

                if (clean)
                    map.CleanTile(newPos);
                else
                {
                    float n = GetNoise(newPos.X * 0.1f, newPos.Y * 0.1f);

                    if (n > max) max = n;
                    if (n < min) min = n;

                    if (n < 0f)
                        map.setTile(newPos, DualTilemap.TileType.WATER);
                    else if (n < 0.1f)
                        map.setTile(newPos, DualTilemap.TileType.SAND);
                    else if (n < 0.4f)
                    {
                        map.setTile(newPos, DualTilemap.TileType.GRASS);
                        if(n > 0.2f && n < 0.3f)
                            plants.SetCell(newPos, 0, new Vector2I((int)GD.RandRange(0, 3), 0));

                    }
                    else
                        map.setTile(newPos, DualTilemap.TileType.DIRT);
                }
            }
        }

        //GD.Print($"Max: {max}, Min: {min}");
    }


    float GetNoise(float x, float z)
    {
        float value = Noise.GetNoise2D(x, z);
        return value;
    }
}
