using Godot;
using Godot.Collections;
using System;
using System.Transactions;

public partial class WorldMap : Node2D
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
    int ChunkRange => Chunk.ChunkRange;
    Vector2I CurrentChunk = Vector2I.MinValue;
    Dictionary<Vector2I, Chunk> Chunks = new Dictionary<Vector2I, Chunk>();

    public override void _Ready()
    {
        OffsetLayer = GetNode<TileMapLayer>("OffsetGrid");
        WorldLayer = GetNode<TileMapLayer>("WorldGrid");
    }

    public void UpdateMap()
    {
        //Vector2I pos = (Vector2I)World.Camera.Position;
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2I pos = WorldLayer.LocalToMap(mousePos);

        GD.Print("########## MAP UPDATE ##########");
        GD.Print("POS: " + pos);
        Vector2I ChunkCoord = PosToChunk(pos);
        GD.Print("CHUNK: " + ChunkCoord);

        Chunk c = new Chunk(ChunkCoord);
        c.Paint();
        /*
        if(ChunkCoord == CurrentChunk)
        {
            GD.Print("SAME CHUNK - NO UPDATE");
            return;
        }

        Array<Vector2I> chunksTmp = new Array<Vector2I>();

        for (int x = ChunkCoord.X - ChunkRange; x <= ChunkCoord.X + ChunkRange; x++)
        {
            for(int y = ChunkCoord.Y - ChunkRange; y <= ChunkCoord.Y + ChunkRange; y++)
            {
                Vector2I coord = new Vector2I(x, y);
                chunksTmp.Add(coord);
            }
        }

        foreach(Vector2I coordRem in Chunks.Keys)
        {
            GD.Print("Remove Chunk: " + coordRem);
            Chunks.Remove(coordRem);
        }

        foreach(Vector2I c in chunksTmp)
        {
            if (!Chunks.ContainsKey(c))
            {
                GD.Print("Add Chunk: " + c);
                Chunks.Add(c, new Chunk(c));
            }
            else
            {
                GD.Print("Keep Chunk: " + c);
            }
        }
        */
    }

    Vector2I PosToChunk(Vector2I pos)
    {
        return new Vector2I(pos.X / Chunk.ChunkSize, pos.Y / Chunk.ChunkSize);
    }
}
