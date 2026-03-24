using Godot;
using System;


public partial class InventarSlot : Control
{
    const int MaxValue = 9;

    public ItemBase Item;
    public int ItemCount;

    public void Update(ItemBase item, int value = 1)
    {
        Sprite2D icon = GetNode("Icon") as Sprite2D;
        if (item != null)
        {
            icon.Texture = item.Texture;

            if (Item != null && Item.ID == item.ID)
            {
                if(ItemCount + value > MaxValue)
                {
                    ItemCount = MaxValue;
                    value = ItemCount + value - 99;
                    
                    //Todo: Drop to Floor
                    
                }
                else
                    ItemCount += value;
            }
            else
            {
                Item = item;
                ItemCount = value;
            }
        }
        else
        {
            icon.Texture = GD.Load("res://assets/items/empty.png") as Texture2D;
            Item = null;
            ItemCount = 0;
        }

        Label l = GetNode<Label>("LabelCount");
        l.Text = ItemCount.ToString();
    }
}
