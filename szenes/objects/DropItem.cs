using Godot;
using System;

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

    private void OnBodyEntered(Node2D body)
    {
        if(body.IsInGroup("Player"))
        {
            ((Player)body).Collect(Item, Amount);

            QueueFree();
        }
    }
}
