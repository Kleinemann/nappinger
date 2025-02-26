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

    public Vector2I off;

    public static Map _Map;

    public Array<Chunk> Chunks = new Array<Chunk>();
    public Chunk CurrentChunk;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _Map = this;

        CurrentChunk = GetChunk(Player.Position);
	}

    void UpdateMap()
    {

    }

    Chunk GetChunk(Vector3 pos)
    {
        Vector3 cPos = ChunkPos(pos);

        foreach(Chunk c in Chunks)
        {
            if (c.Position == cPos)
                return c;
        }

        return CreateChunk(pos);
    }

    Chunk CreateChunk(Vector3 pos)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://test/Chunk.tscn");
        Chunk chunk = scene.Instantiate<Chunk>();

        Vector3 cPos = ChunkPos(pos);

        
        float posX = cPos.X * ChunkWidth;
        float posZ = cPos.Z * ChunkHeigth;

        /*
        if (cPos.X < 0)
            posX -= (ChunkWidth / 2);
        if(cPos.X > 0)
            posX += (ChunkWidth / 2);
        
        if (cPos.Z < 0)
            posZ -= (ChunkHeigth / 2);
        if (cPos.Z > 0)
            posZ += (ChunkHeigth / 2);
        */
        
        chunk.Position = new Vector3((int)posX, 0, (int)posZ);
        chunk.Scale = new Vector3(ChunkWidth, 1, ChunkHeigth);

        Chunks.Add(chunk);
        AddChild(chunk);

        return chunk;
    }

    Vector3 ChunkPos(Vector3 pos) 
    {
        float x = (int)pos.X / ChunkWidth;
        float z = (int)pos.Z / ChunkHeigth;
        Vector3 cPos = new Vector3(x, 0, z);

        return cPos;
    }

	void UpdateChunks()
	{
        CurrentChunk = GetChunk(Player.Position);
        /*
        foreach (Chunk chunk in Chunks.Values)
            chunk.ChunkState = Chunk.ChunkStateEnum.REMOVE;

        for (int x = off.X - ChunkRange; x <= off.X + ChunkRange; x++)
        {
            for (int y = off.Y - ChunkRange; y <= off.Y + ChunkRange; y++)
            {
                Vector2I pos = new Vector2I(x, y);
                if (Chunks.ContainsKey(pos))
                    Chunks[pos].ChunkState = Chunk.ChunkStateEnum.NONE;

                else
                {
                    PackedScene scene = (PackedScene)ResourceLoader.Load("res://test/Chunk.tscn");
                    Chunk chunk = scene.Instantiate<Chunk>();

                    chunk.Position = new Vector3(pos.X, 0, pos.Y);
                    Chunks.Add(pos, chunk);
                    AddChild(chunk);
                }
            }
        }

        var cRem = Chunks.Where(c => c.Value.ChunkState == Chunk.ChunkStateEnum.REMOVE);
        foreach (var c in cRem)
        {
            Chunks.Remove(c.Key);
            c.Value.QueueFree();
        }
        */
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Vector3 cPos = ChunkPos(CurrentChunk.Position);

        Vector3 pPos = ChunkPos(Player.Position);
        if (cPos != pPos)
        {
            UpdateChunks();
        }
    }
}
