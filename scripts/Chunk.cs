using Godot;
using System;

public partial class Chunk : GodotObject
{
    public static readonly int ChunkSize = 16;
    public static readonly int ChunkRange = 3;

    Vector2I Coords;
}
