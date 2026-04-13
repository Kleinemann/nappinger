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

    public static BreakableObject SelectedObject;

    GameObjectDataBase _data = new GameObjectDataBase();

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

    public override void _Ready()
    {
        Area2D area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;
        UpdateAnimation();
    }

    #endregion

    public async Task RIP()
    {
        CollisionShape.Disabled = true;
        //Drop Items
        if (Inventory == null)
            return;


        PackedScene scene = GD.Load<PackedScene>("res://szenes/objects/DropItem.tscn");
        DropItem item = scene.Instantiate<DropItem>();

        item.Position = Position;
        item.Item = Inventory.Items[0].Item;
        item.Amount = Inventory.Items[0].Amount;
        WorldMain.Instance.AddChild(item);

        await ToSignal(GetTree().CreateTimer(5f), "timeout");
        QueueFree();

    }


    public void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Object CLICK");
            Player.SelectetPlayer = null;
            SelectedObject = this;
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
