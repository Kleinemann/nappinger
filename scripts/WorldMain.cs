using Godot;
using System;

public partial class WorldMain : Node2D
{
    public static WorldMain Instance;

    public Camera2D Camera;

    public WorldMap Map;

    public override void _Ready()
    {
        Instance = this;
        Map = GetNode<WorldMap>("DualTileMap");
        Camera = GetNode<Camera2D>("Camera2D");
        Map.UpdateMap();
    }
}
