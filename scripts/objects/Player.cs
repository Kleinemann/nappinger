using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
    AnimatedSprite2D Animator;
    AnimatedSprite2D AnimatorShadow;
    int Speed = 150;
    string direction = "d";
    public static Player SelectetPlayer;

    public static Player GetNextPlayer()
    {
        Array<Node> nodes = WorldMain.Instance.Map.GetChildren();

        Player first = null;
        bool foundCurrent = false;
        foreach (Node n in nodes)
        {
            if (n is Player player)
            {
                if (first == null)
                    first = player;

                if (foundCurrent && player != SelectetPlayer)
                    return player;

                if (player == SelectetPlayer)
                    foundCurrent = true;
            }
        }

        if(first != null)
            return first;

        return null;
    }

    public override void _Ready()
    {
        Area2D area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;
    }


    public void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Player CLICK");
            SelectetPlayer = this;
        }
    }


    public override void _Process(double delta)
    {
        Animator = GetNode<AnimatedSprite2D>("Sprite2D");
        AnimatorShadow = GetNode<AnimatedSprite2D>("Sprite2DShadow");
        PlayerMovement();
        PlayerAnimation();
    }

    public void PlayerMovement()
    {
        Velocity = Input.GetVector("left", "rigth", "up", "down");
        Velocity = Velocity * Speed;

        //TODO: Only in first Person
        if (SelectetPlayer != this)
            Velocity = Vector2.Zero;

        MoveAndSlide();
    }

    public void PlayerAnimation()
    {
        if (Velocity.X > 0) direction = "r";
        else if (Velocity.X < 0) direction = "l";
        else if (Velocity.Y > 0) direction = "d";
        else if (Velocity.Y < 0) direction = "u";

        string animationName;

        if(Velocity == Vector2.Zero)
            animationName = "idle_" + direction;
        else
            animationName= "walk_" + direction;

        Animator.Play(animationName);
        AnimatorShadow.Play(animationName);
    }
}
