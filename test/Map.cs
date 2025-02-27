using Godot;
using Godot.Collections;
using System.Linq;
using System;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

public partial class Map : Node3D
{
	[Export]
	public int Seed = 3;
    [Export]
    public int ChunkRange = 3;
    [Export]    
	public int ChunkWidth = 6;
    [Export]
    public int ChunkHeigth = 6;
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
        chunk.Scale = new Vector3(ChunkWidth, 1, ChunkHeigth);

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
        CurrentChunkPos = ChunkPos(Player.Position); ;

        foreach (Chunk chunk in Chunks.Values)
            chunk.ChunkState = Chunk.ChunkStateEnum.REMOVE;

        for (int x = CurrentChunkPos.X - ChunkRange; x <= CurrentChunkPos.X + ChunkRange; x++)
        {
            for (int y = CurrentChunkPos.Y - ChunkRange; y <= CurrentChunkPos.Y + ChunkRange; y++)
            {
                Vector2I pos = new Vector2I(x, y);
                Chunk c = GetChunk(pos);
                c.ChunkState = Chunk.ChunkStateEnum.NONE;
            }
        }


        var cRem = Chunks.Where(c => c.Value.ChunkState == Chunk.ChunkStateEnum.REMOVE);
        foreach (var c in cRem)
        {
            Chunks.Remove(c.Key);
            c.Value.QueueFree();
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
