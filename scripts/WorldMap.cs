using Godot;
using System;
using System.Collections.Generic;
using System.Transactions;

public partial class WorldMap : Node2D
{
    public FastNoiseLite Noise = new FastNoiseLite()
    {
        NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth,
        Seed = 0,
        Frequency = 0.015f
    };

    public enum TileType { NONE, WATER, SAND, GRASS, DIRT };

    public readonly List<Vector2I> BaseTiles = new List<Vector2I>() { new Vector2I(-1, -1), new Vector2I(2, 1), new Vector2I(6, 1), new Vector2I(10, 1), new Vector2I(14, 1)};
    public readonly Dictionary<TileType, Vector2I> TileTypeCoord = new Dictionary<TileType, Vector2I>()
    {
        { TileType.NONE, new Vector2I(-1,-1) },
        { TileType.WATER, new Vector2I(2,1) },
        { TileType.SAND, new Vector2I(6,1) },
        { TileType.GRASS, new Vector2I(10,1) },
        { TileType.DIRT, new Vector2I(14,1) }
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
        Vector2 gCamera = WorldMain.Instance.Camera.GetViewportRect().Position;
        Vector2 lCamera = ToLocal(gCamera);
        Vector2I ChunkCoord = WorldLayer.LocalToMap(lCamera);


        Vector2I cMouse = GetMouseCoords();
        Vector2I ChunkCoord2 = GetChunkCoords(cMouse);
        //GD.Print("Mouse POS: " + cMouse + " Chunk: " + ChunkCoord);


        //kein Update wenn selber Chunk
        if (ChunkCoord == CurrentChunk)
        {
            //GD.Print("SAME CHUNK - NO UPDATE");
            return;
        }

        CurrentChunk = ChunkCoord;

        List<Vector2I> chunksTmp = new List<Vector2I>();

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
            if(chunksTmp.Contains(coordRem))
                continue;

            //GD.Print("Remove Chunk: " + coordRem);
            Chunks[coordRem].Clean();
            Chunks.Remove(coordRem);
        }

        foreach(Vector2I c in chunksTmp)
        {
            if (!Chunks.ContainsKey(c))
            {
                //GD.Print("Add Chunk: " + c);
                Chunk chunk = new Chunk(c);
                chunk.Paint();
                Chunks.Add(c, chunk);
            }
            else
            {
                //GD.Print("Keep Chunk: " + c);
            }
        }


        //float min = float.MaxValue;
        //float max = float.MinValue;

        //foreach (Chunk chunk in Chunks.Values)
        //{
        //    //chunk.Paint();

        //    if (chunk.MinHeight < min)
        //        min = chunk.MinHeight;

        //    if (chunk.MaxHeight > max)
        //        max = chunk.MaxHeight;
        //}

        //GD.Print("Height Min: " + min + " Max: " + max);

    }

    Vector2I GetMouseCoords()
    {   
        Vector2 gMouse = GetGlobalMousePosition();
        Vector2 lMouse = ToLocal(gMouse);
        return WorldLayer.LocalToMap(lMouse);
    }


    Vector2I GetChunkCoords(Vector2I pos)
    {
        Vector2I negativFix = new Vector2I(0, 0);
        if (pos.X < 0)
        {
            pos.X += 1;
            negativFix.X = -1;
        }
        if (pos.Y < 0)
        {
            negativFix.Y = -1;
            pos.Y += 1;
        }
        return new Vector2I((pos.X / Chunk.ChunkSize), (pos.Y / Chunk.ChunkSize)) + negativFix;
    }
}
