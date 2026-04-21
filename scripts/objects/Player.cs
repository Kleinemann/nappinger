using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public partial class Player : Animal
{
    public Weapon Weapon;
    public Timer ActionTimer;
    public Timer ActionCooldown;
    bool action = false;
    bool cooldown = false;

    Area2D SearchArea;

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

                if (foundCurrent && player != WorldMain.SelectedObject)
                    return player;

                if (player == WorldMain.SelectedObject)
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

        SearchArea = GetNode<Area2D>("Area2DSearch");

        Area2D area = GetNode<Area2D>("Area2D");
        area.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if(Target is Node2D)
        {
            if(body == Target)
            {
                if(body.IsInGroup("Breakable"))
                    State = GameObjectState.FIGHTING;

                if (body.IsInGroup("Storable"))
                {
                    Target = null;
                    State = GameObjectState.IDLE;
                }
            }
        }
    }

    public void OnTimerTimeout()
    {
        Weapon.Hide();
        Weapon.CollisionShape.Disabled = true;
        action = false;
        cooldown = true;
        ActionCooldown.Start();
    }

    public void OnTimerCoolDownTimeout()
    {
        cooldown = false;
    }

    public new void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Player CLICK");
            WorldMain.SelectedObject = this;
        }
    }


    public override void _Process(double delta)
    {
        PlayerWeapon();        
        Movement();

        if (!action)
        {
            UpdateAnimation();
        }

        if(State == GameObjectState.IDLE)
        {
            SetNextState();
        }
    }

    public void SetSearch(string search)
    {
        if(search != null)
        {
            Node2D target = SearchNextResource(search);
            SetTarget(target);
        }
        else
            State = GameObjectState.IDLE;
    }


    void SetNextState()
    {
        if(State == GameObjectState.WAITING)
        {
            State = GameObjectState.IDLE;
                return;
        }

        if(Mission != null)
        {
            if(Mission.State == GameObjectState.FARMING)
            {
                string search = (string)Mission.Target;

                Node2D target;
                if(Inventory.CountItemGroup(search) == 0)
                    target = SearchNextResource(search);
                else
                {
                    target = SearchNextResource("Storable");
                }

                State = GameObjectState.WALKING;
                Target = target;
            }
        }
    }

    Node2D SearchNextResource(string groupName)
    {
        var objs = SearchArea.GetOverlappingBodies();
        objs.AddRange(SearchArea.GetOverlappingAreas());

        Array<Node2D> nodes = new Array<Node2D>();

        foreach (Node2D obj in objs)
        {
            if (obj.IsInGroup(groupName))
            {
                nodes.Add(obj);
            }
        }

        Node2D nearest = null;
        float min = float.MaxValue;
        foreach (Node2D obj in nodes)
        {
            float dist = Position.DistanceTo(obj.Position);
            if(dist < min)
            {
                min = dist;
                nearest = obj;
            }
        }

        return nearest;
    }


    public void PlayerWeapon()
    {
        if(!action && !cooldown 
            && ((WorldMain.SelectedPlayer == this && Input.IsActionJustPressed("attack"))
                    || State == GameObjectState.FIGHTING))
        {
            Weapon.Show();
            Animator.Play("attack_" + Direction);
            AnimatorShadow.Play("attack_" + Direction);
            Weapon.CollisionShape.Disabled = false;
            ActionTimer.Start();
            action = true;
        }
    }


    public void Collect(InventoryItem item, int amount = 1)
    {
        Inventory.Insert(item, amount);
    }

    internal void SetTarget(Vector2 vector2)
    {
        State = GameObjectState.WALKING;
        Target = vector2;
    }

    internal void SetTarget(Node2D node)
    {
        Target = node;
        State = node == null ? GameObjectState.IDLE : GameObjectState.WALKING;

        if(node == null && !Inventory.IsEmpty)
        {
            Node2D nearest = null;
            float min = float.MaxValue;
            var storages = GetTree().GetNodesInGroup("Storable");
            foreach (Node2D store in storages)
            {
                float dist = Position.DistanceTo(store.Position);
                if(dist < min)
                {
                    min = dist;
                    nearest = store;
                }
            }

            SetTarget(nearest);
        }
    }

    

    public override void Movement()
    {
        Velocity = Vector2.Zero;

        //TODO: Only in first Person
        if (WorldMain.SelectedPlayer == this)
            Velocity = Input.GetVector("left", "rigth", "up", "down");

        if (State == GameObjectState.WALKING && Target != null)
        {
            Vector2 targetPos =  Target is Vector2 ? (Vector2)Target : ((Node2D)Target).Position;

            Vector2 direction = (targetPos - GlobalPosition).Normalized();
            Velocity = direction;
        }

        Velocity *= Speed;

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
