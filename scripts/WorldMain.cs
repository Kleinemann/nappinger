using Godot;
using nappinger.scripts;
using System.Data;

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
        Ui.Instance.Update();
    }


    public override void _Process(double delta)
    {
        Time += delta;
        if(Time >= 1)
        {
            Time = 0;
            foreach (Chunk chunk in Map.Chunks.Values)
            {
                chunk.Process();
            }
        }

        UpdateMouseIcon();
    }

    public void UpdateMouseIcon()
    {
        if (Map.Marker.CurrentItem != null)
        {
            if (Map.Marker.CurrentItem.ItemType == ItemTypeEnum.PLAYER)
            {
                Vector2I mouse = Map.GetMouseCoords();
                if (Map.ItemLayer.GetCellSourceId(mouse) >= 0)
                {
                    TileData data = Map.ItemLayer.GetCellTileData(mouse);
                    ItemTypeEnum type = (ItemTypeEnum)((int)data.GetCustomData("ItemType"));

                    Resource res = null;
                    switch (type)
                    {
                        case ItemTypeEnum.PLANT:
                            res = GD.Load("res://assets/actions/lumber.png");
                            break;

                        case ItemTypeEnum.ANIMAL:
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GameItem item = Map.GetItem();
            if(item != null)
                Map.Marker.Select(item);
            else                
                Map.Marker.Deselect();
        }

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            if(Map.Marker.CurrentItem != null && Map.Marker.CurrentItem.ItemType == ItemTypeEnum.PLAYER)
            {
                GameItemMoveable gim = (GameItemMoveable)Map.Marker.CurrentItem;
                gim.ItemState = ItemStateEnum.WALKING;

                GameItem gi = Map.GetItem();

                if (Map.GetItem() != null)
                {
                    gim.TargetItem = gi;
                    gim.TargetPosition = null;
                }
                else
                {
                    gim.TargetPosition = Map.GetMouseCoords();
                    gim.TargetItem = null;
                }
            }
        }

        @event.Dispose();
    }
}
