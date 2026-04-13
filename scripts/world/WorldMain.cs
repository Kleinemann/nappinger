using Godot;
using System;

public partial class WorldMain : Node2D
{
    public static WorldMain Instance;
    public CameraScroll Camera;
    public WorldMap Map;
    public double Time = 0;

    public static RandomNumberGenerator Random = new RandomNumberGenerator();

    public override void _Ready()
    {
        Instance = this;

        Random.Randomize();

        Map = GetNode<WorldMap>("DualTileMap");
        Camera = GetNode<CameraScroll>("Camera2D");

        Map.UpdateMap();

        //select first player
        Player.SelectetPlayer = Player.GetNextPlayer();
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
                Player.SelectetPlayer = p;
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
            Player.SelectetPlayer = null;
            BreakableObject.SelectedObject = null;
        }

        //targeting
        if (Input.IsMouseButtonPressed(MouseButton.Right))
        { 
            if(Player.SelectetPlayer != null)
            {
                Player.SelectetPlayer.Target = Map.GetGlobalMousePosition();
            }
        }

        if (@event.IsActionPressed("add_value"))
        {
            if(Player.SelectetPlayer != null)
                Player.SelectetPlayer.Healt++;
            else if(BreakableObject.SelectedObject != null)
                BreakableObject.SelectedObject.Healt++;
        }

        if (@event.IsActionPressed("remove_value"))
        {
            if(Player.SelectetPlayer != null)
                Player.SelectetPlayer.Healt--;
            else if(BreakableObject.SelectedObject != null)
                BreakableObject.SelectedObject.Healt--;
        }

        @event.Dispose();        
    }
}

