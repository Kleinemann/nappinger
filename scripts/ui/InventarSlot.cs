using Godot;
using System;


public partial class InventarSlot : Control
{
    public ItemBase Item;

    public void Update(ItemBase item)
    {
        int count;
        Sprite2D icon = GetNode("Icon") as Sprite2D;
        if (item != null)
        {
            Item = item;
            icon.Texture = item.Texture;
            count = item.Count;
        }
        else
        {
            icon.Texture = GD.Load("res://assets/items/empty.png") as Texture2D;
            Item = null;
            count = 0;
        }

        Label l = GetNode<Label>("LabelCount");
        l.Text = count > 1 ? count.ToString() : "";
    }
}
