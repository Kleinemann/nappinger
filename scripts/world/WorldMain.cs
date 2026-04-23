using Godot;
using System;
using System.Text.RegularExpressions;

public partial class WorldMain : Node2D
{
    public static WorldMain Instance;
    public CameraScroll Camera;
    public WorldMap Map;
    public double Time = 0;

    //Ausgewähltes Object
    public static Node2D SelectedObject;
    public static Player SelectedPlayer => SelectedObject != null && SelectedObject is Player player ? player : null;
    public static Animal SelectedAnimal => SelectedObject != null && SelectedObject is Animal animal ? animal : null;
    public static BreakableObject SelectedBreakable => SelectedObject != null && SelectedObject is BreakableObject breakable ? breakable : null;
    public static Store SelectedStore => SelectedObject != null && SelectedObject is Store store ? store : null;

    public static RandomNumberGenerator Random = new RandomNumberGenerator();

    public override void _Ready()
    {
        Instance = this;

        Random.Randomize();

        Map = GetNode<WorldMap>("DualTileMap");
        Camera = GetNode<CameraScroll>("Camera2D");

        Vector2 v2 = Tools.GetGeoLocation();
        Vector2I coord = new Vector2I((int)v2.X * 10, (int)v2.Y * 10);
        Camera.Position = Map.CoordsToPosition(coord);

        Map.UpdateMap();

        InitPlayerStartup();
    }

    public void InitPlayerStartup()
    {
        PackedScene scene = GD.Load<PackedScene>("res://szenes/buildings/camp.tscn");
        Store camp = scene.Instantiate<Store>();

        InventoryItem item = ResourceLoader.Load<InventoryItem>("res://resources/items/food.tres");
        camp.Inventory.Items[0].Item = item;
        camp.Inventory.Items[0].Amount = 10;

        camp.Position = Camera.Position;
        Map.AddChild(camp);

        //select first player
        SelectedObject = Player.GetNextPlayer();
    }


    public override void _Process(double delta)
    {
        Hud.Instance.UpdateHud();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //Select next player
        if (@event.IsActionPressed("next_player"))
        {
            Player p = Player.GetNextPlayer();
            if (p != null)
            {
                SelectedObject = p;
                Camera.CameraTarget = p;
            }
        }

        //Quit
        if (@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        //Focus / defocus Player
        if (@event.IsActionPressed("camera_focus"))
        {
            Camera.SwitchFocus();
        }

        //Deselect 
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("WORLD CLICK");
            WorldMain.SelectedObject = null;
        }

        //targeting
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        { 
            SelectedPlayer?.SetTarget(Map.GetGlobalMousePosition());
        }

        if (@event.IsActionPressed("add_value"))
        {
            if(SelectedAnimal != null)
                SelectedAnimal.Healt++;
            else if(SelectedBreakable != null)
                SelectedBreakable.Healt++;
        }

        if (@event.IsActionPressed("remove_value"))
        {
            if(SelectedAnimal != null)
                SelectedAnimal.Healt--;
            else if(SelectedBreakable != null)
                SelectedBreakable.Healt--;
        }

        if(@event.IsActionPressed("ControlCenter"))
        {
            var nodes = GetTree().GetNodesInGroup("Storable");
            //TODO: dound the nearest
            if (nodes.Count > 0)
                WorldMain.SelectedObject = (Node2D)nodes[0];

            Hud.Instance.SwitchPlayerControlCenter();
        }

        @event.Dispose();        
    }
}

