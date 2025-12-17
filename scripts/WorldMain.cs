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
        //Map.UpdateMap();

        Chunk chunk3 = new Chunk(new Vector2I(-1, -1));
        Chunk chunk1 = new Chunk(new Vector2I(0, 0));
        Chunk chunk2 = new Chunk(new Vector2I(1, 1));

        chunk1.Paint();
        chunk2.Paint();
        chunk3.Paint();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Map.UpdateMap();
            @event.Dispose();
        }
    }
}
