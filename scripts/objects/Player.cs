using Godot;
using Godot.Collections;

public partial class Player : CharacterBody2D
{
    AnimatedSprite2D Animator;
    AnimatedSprite2D AnimatorShadow;
    string direction = "d";
    public object Target;

    public static Player SelectetPlayer;

    #region GameObjectData
    public GameObjectDataMoveable _data = new GameObjectDataMoveable();

    [Export]
    public string ObjectName
    {
        get => _data.Name;
        set => _data.Name = value;
    }

    [Export]
    public Texture2D Icon
    {
        get => _data.Icon;
        set => _data.Icon = value;
    }

    [Export]
    public int Healt
    {
        get => _data.Healt;
        set
        {
            _data.Healt = value;
            if (_data.Healt <= 0)
            {
                _data.Healt = 0;
                State = GameObjectState.DEAD;
            }
            if (Healt > MaxHealt)
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

    [Export]
    public Inventory Inventory
    {
        get => _data.Inventory;
        set => _data.Inventory = value;
    }
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

    public void Collect(InventoryItem item, int amount = 1)
    {
        Inventory.Insert(item, amount);
    }


    public void PlayerMovement()
    {
        Velocity = Vector2.Zero;

        //TODO: Only in first Person
        if (SelectetPlayer == this)
            Velocity = Input.GetVector("left", "rigth", "up", "down");

        if (Target != null && Target is Vector2 targetPos)
        {
            if(targetPos.DistanceTo(GlobalPosition) > 2)
            {
                Vector2 direction = (targetPos - GlobalPosition).Normalized();
                Velocity = direction;
            }
            else
                Target = null;
        }

        Velocity = Velocity * Speed;

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
