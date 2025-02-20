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
	public int ChunkWidth = 5;
    [Export]
    public int ChunkHeigth = 5;
	[Export]
	public TPlayer Player;

    public Vector2I off;


    public Dictionary<Vector2I, Chunk> Chunks = new Dictionary<Vector2I, Chunk>();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	void UpdateChunks()
	{
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
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Vector2I o = new Vector2I(((int)Player.Position.X), ((int)Player.Position.Z));
        if (o != off)
        {
            off = o;
            UpdateChunks();
        }
    }
}
