using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BreakableObject : StaticBody2D
{
    [Export] public AnimatedSprite2D Sprite;
    [Export] public AnimatedSprite2D SpriteShadow;
    [Export] public CollisionShape2D CollisionShape;
    [Export] public ProgressBar HealtBar;

    GameObjectDestoyable _data = new GameObjectDestoyable();
    Timer _timer;

    #region GameObjectData
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
                _ = RIP();
            }

            if(_data.Healt > MaxHealt)
                _data.Healt = MaxHealt;

            this.UpdateAnimation();
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
    public Inventory Inventory
    {
        get => _data.Inventory;
        set => _data.Inventory = value;
    }

    #endregion

    public override void _Ready()
    {
        Area2D area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;

        _timer = GetNode<Timer>("RegenTimer");
        if (_timer != null)
        {
            _timer.OneShot = true;
            _timer.WaitTime = 30;
            _timer.Timeout += Respawn;
        }

        UpdateAnimation();
    }

    private void Respawn()
    {
        Healt = MaxHealt;
        UpdateAnimation();
        CollisionShape.Disabled = false;
        CollisionLayer = 1;
    }

    public async Task RIP()
    {
        GameObjectDataMoveable.RemoveFromTarget(this);
        CollisionShape.Disabled = true;
        CollisionLayer = 0;
        //Drop Items
        if (Inventory != null)
        {
            DropItem item = DropItem.CreateDropItem(Inventory.Items[0].Item, Inventory.Items[0].Amount);
            item.Position = Position;
            WorldMain.Instance.AddChild(item);
        }
        if(_timer == null)
            QueueFree();
        else
        {
            _timer.Start();
        }
    }


    public void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Object CLICK");
            WorldMain.SelectedObject = this;
        }
    }


    public void UpdateAnimation()
    {
        if (Sprite == null)
            return;

        float healt = (Healt * 100 / MaxHealt);

        HealtBar.Visible = Healt != MaxHealt && Healt > 0;
        HealtBar.Value = Healt;
        HealtBar.MaxValue = MaxHealt;

        if (healt == 0) Sprite.Play("3");
        else if (healt < 40) Sprite.Play("2");
        else if (healt < 70) Sprite.Play("1");
        else Sprite.Play("0");

        if (healt == 0) SpriteShadow.Play("3");
        else if (healt < 40) SpriteShadow.Play("2");
        else if (healt < 70) SpriteShadow.Play("1");
        else SpriteShadow.Play("0");

    }
}
