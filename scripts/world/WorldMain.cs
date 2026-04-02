using Godot;

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
    }


    public override void _Process(double delta)
    {

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //Quit
        if(@event.IsActionPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        //Focus / defocus Player
        if(@event.IsActionPressed("camera_focus"))
        {
            Camera.SwitchFocus();
        }

        //Deselect 
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("WORLD CLICK");
            Player.SelectetPlayer = null;
        }

 
        @event.Dispose();
        
    }
}

