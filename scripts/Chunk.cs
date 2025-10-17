using Godot;
using System;

public partial class Chunk : GodotObject
{
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkRange = 3;

    public Vector2I Coords;

    public Chunk()
    {
    }

    public Chunk(Vector2I coords)
    {
        Coords = coords;
    }
}
