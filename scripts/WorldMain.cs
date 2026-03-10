using Godot;
using nappinger.scripts;
using System;
using System.Collections.Generic;
using static DualTilemap;

public partial class WorldMain : Node2D
{
    public static WorldMain Instance;
    public Camera2D Camera;
    public WorldMap Map;
    public double Time = 0;

    public static RandomNumberGenerator Random = new RandomNumberGenerator();

    public override void _Ready()
    {
        Instance = this;

        Random.Randomize();

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
            Map.Marker.Update();
        }

        if (Map.Marker.CurrentItem != null)
        {
            if (Map.Marker.CurrentItem.Type == ItemType.PLAYER)
            {
                Vector2I mouse = Map.GetMouseCoords();
                if (Map.ItemLayer.GetCellSourceId(mouse) >= 0)
                {
                    TileData data = Map.ItemLayer.GetCellTileData(mouse);
                    ItemType type = (ItemType)((int)data.GetCustomData("ItemType"));

                    Resource res = null;
                    switch (type)
                    {
                        case ItemType.PLANT:
                            res = GD.Load("res://assets/actions/lumber.png");
                            break;

                        case ItemType.ANIMAL:
                            res = GD.Load("res://assets/actions/hunt.png");
                            break;

                        default:

                            break;
                    }

                    Input.SetCustomMouseCursor(res);
                }
                else
                {
                    Input.SetCustomMouseCursor(null);
                }
            }
        }
    }


    public void ProcessChunks()
    {
        foreach (Chunk chunk in Map.Chunks.Values)
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
            //Select tile
            Vector2I pos = Map.GetMouseCoords();
            bool marked = Map.Marker.Select(pos);

            @event.Dispose();
        }

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            if(Map.Marker.Visible && Map.Marker.CurrentItem != null && Map.Marker.CurrentItem is GameItemMoveable)
            {
                GameItemMoveable gim = (GameItemMoveable)Map.Marker.CurrentItem;
                gim.State = ItemState.WALKING;

                Vector2I pos = Map.GetMouseCoords();
                if (Map.ItemLayer.GetCellSourceId(pos) >= 0)
                {
                    Chunk chunk = Map.GetChunk(pos);                    
                    gim.TargetItem = chunk.Items[pos];
                }
                else
                    gim.TargetPosition = Map.GetMouseCoords();
            }

            @event.Dispose();
        }
    }
}
