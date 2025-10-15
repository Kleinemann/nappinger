using Godot;
using Godot.Collections;
using System;

public partial class WorlMap : Node2D
{
    public FastNoiseLite Noise = new FastNoiseLite()
    {
        NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth,
        Seed = 0,
        Frequency = 0.1689f
    };

    public TileMapLayer OffsetLayer;
    public TileMapLayer WorldLayer;

    WorldMain World => WorldMain.Instance;

    Dictionary<Vector2I, Chunk> Chunks = new Dictionary<Vector2I, Chunk>();

    public override void _Ready()
    {
        UpdateMap();
    }

    public void UpdateMap()
    {
        Vector2I pos = (Vector2I)World.Camera.Position;
        Vector2I ChunkCoord = PosToChunk(pos);
        GD.Print(ChunkCoord);
    }

    Vector2I PosToChunk(Vector2I pos)
    {
        return new Vector2I(pos.X / Chunk.ChunkSize, pos.Y / Chunk.ChunkSize);
    }
}
