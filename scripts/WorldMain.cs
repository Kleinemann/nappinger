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
    Sprite2D SpriteW;
    Sprite2D SpriteF;
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
        if (Map.Marker.CurrentObject != null)
        {
            if (Map.Marker.CurrentObject.ObjectType == ObjectTypeEnum.PLAYER)
            {
                Vector2I mouse = Map.GetMouseCoords();
                if (Map.ObjectLayer.GetCellSourceId(mouse) >= 0)
                {
                    TileData data = Map.ObjectLayer.GetCellTileData(mouse);
                    ObjectTypeEnum type = (ObjectTypeEnum)((int)data.GetCustomData("ItemType"));

                    Resource res = null;
                    switch (type)
                    {
                        case ObjectTypeEnum.PLANT:
                            res = GD.Load("res://assets/actions/lumber.png");
                            break;

                        case ObjectTypeEnum.ANIMAL:
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
        if(@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.I)
        {
            if (SpriteW == null)
            {
                ItemBase item = ItemBase.NewItem(1);
                item.Position = new Vector2I(100, 100);
                SpriteW = item;
                AddChild(item);
            }
            else
            {
                ((ItemBase)SpriteW).Value++;
            }
        }

        if (@event is InputEventKey keyEventF && keyEventF.IsPressed() && keyEventF.Keycode == Key.O)
        {
            if (SpriteF == null)
            {
                ItemBase item = ItemBase.NewItem(2);
                item.Position = new Vector2I(140, 100);
                SpriteF = item;
                AddChild(item);
            }
            else
            {
                ((ItemBase)SpriteF).Value++;
            }
        }

        if (@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GameObject go = Map.GetItem();
            if(go != null)
                Map.Marker.Select(go);
            else                
                Map.Marker.Deselect();
        }

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            if(Map.Marker.CurrentObject != null && Map.Marker.CurrentObject.ObjectType == ObjectTypeEnum.PLAYER)
            {
                GameItemMoveable gim = (GameItemMoveable)Map.Marker.CurrentObject;
                gim.ObjectState = ObjectStateEnum.WALKING;

                GameObject gi = Map.GetItem();

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

