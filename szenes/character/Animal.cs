using Godot;
using System;
using System.Threading.Tasks;

public partial class Animal: CharacterBody2D
{
    [Export] public AnimatedSprite2D Sprite;
    [Export] public ProgressBar HealtBar;

    public AnimatedSprite2D Animator;
    public AnimatedSprite2D AnimatorShadow;

    public int moveCounter = 0;

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
            {
                GD.Print(Name + " is dead");
                RIP();
            }
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

    public string Direction
    {
        get => _data.Direction;
        set => _data.Direction = value;
    }

    public object Target
    {
        get => _data.Target;
        set => _data.Target = value;
    }

    #endregion

    public override void _Process(double delta)
    {
        Movement();
        UpdateAnimation();
    }


    public override void _Ready()
    {
        Animator = GetNode<AnimatedSprite2D>("Sprite2D");
        AnimatorShadow = GetNode<AnimatedSprite2D>("Sprite2DShadow");

        Area2D area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;

        UpdateAnimation();
    }

    public void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Animal CLICK");
            WorldMain.SelectedObject = this;
        }
    }

    public void RIP()
    {
        //Drop Items
        if (Inventory != null)
        {

            PackedScene scene = GD.Load<PackedScene>("res://szenes/objects/DropItem.tscn");
            DropItem item = scene.Instantiate<DropItem>();

            item.Position = Position;
            item.Item = Inventory.Items[0].Item;
            item.Amount = Inventory.Items[0].Amount;
            WorldMain.Instance.AddChild(item);
        }
        QueueFree();

    }

    public virtual void Movement()
    {
        if(Target == null)
        {
            float randomX = WorldMain.Random.RandfRange(-50, 50);
            float randomY = WorldMain.Random.RandfRange(-50, 50);
            Target = Position + new Vector2(randomX, randomY);
        }

        if (((Vector2)Target).DistanceTo(GlobalPosition) > 2)
        {
            Vector2 direction = ((Vector2)Target - GlobalPosition).Normalized();
            Velocity = direction;
            Velocity *= Speed;
        }
        else
        { 
            Target = null;
            return;
        }
        
        if(!MoveAndSlide())
            moveCounter++;
        
        if (moveCounter > 10)
        {
            moveCounter = 0;
            Target = null;
        }
    }


    public virtual void UpdateAnimation()
    {
        if(HealtBar != null) 
        {
            HealtBar.Visible = Healt != MaxHealt && Healt > 0;
            HealtBar.Value = Healt;
            HealtBar.MaxValue = MaxHealt;
        }


        if (Velocity.X > 0) Direction = "r";
        else if (Velocity.X < 0) Direction = "l";
        else if (Velocity.Y > 0) Direction = "d";
        else if (Velocity.Y < 0) Direction = "u";

        string animationName;

        if (Velocity == Vector2.Zero)
            animationName = "idle_" + Direction;
        else
            animationName = "walk_" + Direction;

        Animator.Play(animationName);
        AnimatorShadow.Play(animationName);
    }
}
