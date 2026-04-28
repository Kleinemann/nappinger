using Godot;
using System;

public partial class Store : StaticBody2D
{
    GameObjectDataBase _data = new GameObjectDataBase();

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
    public Inventory Inventory
    {
        get => _data.Inventory;
        set => _data.Inventory = value;
    }

    public override void _Ready()
    {
        Area2D area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;

        area.BodyEntered += Area_BodyEntered;
    }

    private void Area_BodyEntered(Node2D body)
    {
        if(body is Player player)
        {
            Inventory iPlayer = player.Inventory;
            
            for(int i = 0; i < iPlayer.Slots.Count; i++)
            {
                var item = iPlayer.Slots[i];

                if (item != null)
                {
                    Inventory.Insert(item.Item, item.Amount);
                }
                iPlayer.Slots[i].Item = null;
                iPlayer.Slots[i].Amount = 0;
            }
        }
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Store CLICK");
            WorldMain.SelectedObject = this;

            Hud.Instance.SwitchPlayerControlCenter();
        }
    }
}
