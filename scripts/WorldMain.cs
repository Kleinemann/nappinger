using Godot;
using System;
using static DualTilemap;

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

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Map.UpdateMap();
            @event.Dispose();
        }
    }
}
