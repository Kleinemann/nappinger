using Godot;
using System;

public partial class DropItem : Area2D
{
    [Export]
    public ItemBase Item;

    public override void _Ready()
    {
        if (Item == null)
            Item = GD.Load<ItemBase>("res://resources/items/Wood.tres");
        Item.Count = 2;

        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");

        sprite.Texture = Item.Texture; 
    }
}
