using Godot;
using System;
using System.Collections.Generic;

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

    List<Chunk> Chunks = new List<Chunk>();

    public override void _Ready()
    {
        UpdateMap();
    }

    public void UpdateMap()
    {
        Vector2 pos = World.Camera.Position;

    }
}
