using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
    AnimatedSprite2D Animator;
    AnimatedSprite2D AnimatorShadow;
    string direction = "d";
    public static Player SelectetPlayer;

    #region GameObjectData
    [Export] 
    public string ObjectName
    {
        get => _data.Name;
        set => _data.Name = value;
    }

    [Export]
    public int Healt
    { get => _data.Healt;
        set
        {
            _data.Healt = value;
            if (_data.Healt <= 0)
            {
                _data.Healt = 0;
                State = GameObjectState.DEAD;
            }
            if(Healt > MaxHealt)
                _data.Healt = MaxHealt;
        }
    }

    [Export] 
    public int MaxHealt
    {
        get => _data.MaxHealt;
        set
        {
            _data.MaxHealt = value;
            if (_data.MaxHealt < 1)
                _data.MaxHealt = 1;
        }
    }

    [Export]
    public GameObjectState State
    {
        get => _data.State;
        set
        {
            _data.State = value;
            if (_data.State == GameObjectState.DEAD)
                GD.Print("Player is dead");
        }
    }

    [Export] 
    public float Speed
    {
        get => _data.Speed;
        set => _data.Speed = value;
    }

    GameObjectDataMoveable _data = new GameObjectDataMoveable();
    #endregion

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
            BreakableObject.SelectedObject = null;
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
