using Godot;
using System;

public partial class WorldMain : Node2D
{
    public static WorldMain Instance;

    public Camera2D Camera;

    public WorlMap Map;

    public override void _Ready()
    {
        Instance = this;
        Camera = GetNode<Camera2D>("Camera2D");
    }
}
