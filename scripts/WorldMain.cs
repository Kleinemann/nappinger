using Godot;
using System;
using System.Collections.Generic;
using static DualTilemap;

public partial class WorldMain : Node2D
{
    public static WorldMain Instance;
    public Camera2D Camera;
    public WorldMap Map;
    public double Time = 0;

    public override void _Ready()
    {
        Instance = this;
        Map = GetNode<WorldMap>("DualTileMap");
        Camera = GetNode<Camera2D>("Camera2D");

        Map.UpdateMap();
    }


    public override void _Process(double delta)
    {
        Time += delta;
        if(Time >= 1)
        {
            Time = 0;
            ProcessChunks();
        }
    }


    public void ProcessChunks()
    {
        foreach(Chunk chunk in Map.Chunks.Values)
        {
            chunk.Process();
        }
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
