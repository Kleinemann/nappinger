using Godot;
using System;
using System.Linq;

public partial class DropItem : Area2D
{
    [Export] public InventoryItem Item;
    [Export] public int Amount = 1;

    public override void _Ready()
    {
        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
        if (Item != null && Item.Icon != null)
        {
            sprite.Texture = Item.Icon;
        }

        BodyEntered += OnBodyEntered;
    }

    public static DropItem CreateDropItem(string resourceName, int amount = 1)
    {
        InventoryItem invItem = InventoryItem.CreateInventoryItem(resourceName);
        return CreateDropItem(invItem, amount);
    }

    public static DropItem CreateDropItem(InventoryItem invItem, int amount = 1)
    {
        PackedScene scene = GD.Load<PackedScene>("res://szenes/objects/DropItem.tscn");
        DropItem item = scene.Instantiate<DropItem>();

        item.Item = invItem;
        item.Amount = amount;

        return item;
    }

    public override void _EnterTree()
    {
        AddToGroup(Item.GroupName);
        Label label = GetNode<Label>("Label");
        label.Text = Amount.ToString();

        label.Visible = Amount > 1;
    }

    private void OnBodyEntered(Node2D body)
    {
        if(body.IsInGroup("Player"))
        {
            ((Player)body).Collect(Item, Amount);

            GameObjectDataMoveable.RemoveFromTarget(this);
            QueueFree();
        }
    }
}
