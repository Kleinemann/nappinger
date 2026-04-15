using Godot;
using Godot.Collections;

public partial class Player : Animal
{
    //public static Player SelectetPlayer;

    //AnimatedSprite2D Animator;
    //AnimatedSprite2D AnimatorShadow;

    public Weapon Weapon;
    public Timer ActionTimer;
    public Timer ActionCooldown;
    bool cooldown = false;

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

                if (foundCurrent && player != SelectetAnimal)
                    return player;

                if (player == SelectetAnimal)
                    foundCurrent = true;
            }
        }

        if(first != null)
            return first;

        return null;
    }

    public override void _Ready()
    {
        base._Ready();
        Weapon = GetNode<Weapon>("Weapon");

        ActionTimer = GetNode<Timer>("ActionTimer");
        ActionTimer.Timeout += OnTimerTimeout;

        ActionCooldown = GetNode<Timer>("ActionCooldown");
        ActionCooldown.Timeout += OnTimerCoolDownTimeout;
    }

    public void OnTimerTimeout()
    {
        Weapon.Hide();
        Weapon.CollisionShape.Disabled = true;
        State = GameObjectState.IDLE;
        cooldown = true;
        ActionCooldown.Start();
    }

    public void OnTimerCoolDownTimeout()
    {
        cooldown = false;
    }

    public void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Player CLICK");
            BreakableObject.SelectedObject = null;
            SelectetAnimal = this;
        }
    }


    public override void _Process(double delta)
    {
        PlayerWeapon();        
        Movement();

        if (State != GameObjectState.FIGHTING)
        {
            UpdateAnimation();
        }
    }

    public void PlayerWeapon()
    {
        if(SelectetAnimal == this && !cooldown 
            && Input.IsActionJustPressed("attack") 
            && State != GameObjectState.FIGHTING)
        {
            State = GameObjectState.FIGHTING;
            Weapon.Show();
            Animator.Play("attack_" + Direction);
            AnimatorShadow.Play("attack_" + Direction);
            Weapon.CollisionShape.Disabled = false;
            ActionTimer.Start();
        }
    }


    public void Collect(InventoryItem item, int amount = 1)
    {
        Inventory.Insert(item, amount);
    }


    public override void Movement()
    {
        Velocity = Vector2.Zero;

        //TODO: Only in first Person
        if (SelectetAnimal == this)
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

    public override void UpdateAnimation()
    {
        if (Velocity.X > 0) Direction = "r";
        else if (Velocity.X < 0) Direction = "l";
        else if (Velocity.Y > 0) Direction = "d";
        else if (Velocity.Y < 0) Direction = "u";

        string animationName;

        if(Velocity == Vector2.Zero)
            animationName = "idle_" + Direction;
        else
            animationName= "walk_" + Direction;

        Animator.Play(animationName);
        AnimatorShadow.Play(animationName);
    }
}
