using Godot;
using Godot.Collections;
using System.Linq;
using System;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
//using System.Collections.Generic;

//[Tool]
public partial class Map : Node3D
{
	[Export]
	public int Seed = 3;
    [Export]
    public FastNoiseLite Noise = new FastNoiseLite();

    [Export]
    public int ChunkRange = 3;
    [Export]    
	public int ChunkWidth = 16;
    [Export]
    public int ChunkHeigth = 16;
    [Export]
    public int ChunkResolution = 1;
    [Export]
    public int VertexMulti = 10;
    [Export]
	public TPlayer Player;


    public static Map _Map;

    public Dictionary<Vector2I, Chunk> Chunks = new Dictionary<Vector2I, Chunk>();
    public Vector2I CurrentChunkPos;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _Map = this;

        UpdateChunks();
	}

    void UpdateMap()
    {

    }

    Chunk GetChunk(Vector2I pos)
    {
        if(Chunks.ContainsKey(pos))
            return Chunks[pos];

        return CreateChunk(pos);
    }

    Chunk CreateChunk(Vector2I p)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://test/Chunk.tscn");
        Chunk chunk = scene.Instantiate<Chunk>();

        float posX = p.X * ChunkWidth;
        float posZ = p.Y * ChunkHeigth;

        chunk.Position = new Vector3((int)posX, 0, (int)posZ);

        Chunks.Add(p, chunk);
        AddChild(chunk);

        return chunk;
    }

    Vector2I ChunkPos(Vector3 pos) 
    {
        int x = (int)pos.X / ChunkWidth;
        int z = (int)pos.Z / ChunkHeigth;
        Vector2I cPos = new Vector2I(x, z);

        return cPos;
    }

    void UpdateChunks()
    {
        CurrentChunkPos = ChunkPos(Player.Position);

        Array<Vector2I> tmp = new Array<Vector2I>();
        tmp.AddRange(Chunks.Keys);

        for (int x = CurrentChunkPos.X - ChunkRange; x <= CurrentChunkPos.X + ChunkRange; x++)
        {
            for (int y = CurrentChunkPos.Y - ChunkRange; y <= CurrentChunkPos.Y + ChunkRange; y++)
            {
                Vector2I pos = new Vector2I(x, y);

                if (tmp.Contains(pos))
                {
                    tmp.Remove(pos);
                }
                else
                {
                    Chunk c = CreateChunk(pos);
                }
            }
        }

        foreach(Vector2I v in tmp) 
        {
            Chunks[v].QueueFree();
            Chunks.Remove(v);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Vector2I pPos = ChunkPos(Player.Position);
        if (CurrentChunkPos != pPos)
        {
            UpdateChunks();
        }
    }
}
