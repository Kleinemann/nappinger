using Godot;
using System;

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

        Map.UpdateMap();

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

        @event.Dispose();        
    }
}

