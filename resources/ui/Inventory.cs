using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class Inventory : Resource
{
    [Export] public Array<InventorySlot> Items = new Array<InventorySlot>();
    [Export] public int Slots = 10;
    [Export] public int MaxStackSize = 10;

    public Inventory()
    {
        for (var i = 0; i < Slots; i++)
        {
            InventorySlot slot = new InventorySlot() { ResourceLocalToScene = true }; 
            Items.Add(slot);
        }
    }

    public bool IsEmpty
    {
        get
        {
            foreach (var slot in Items)
            {
                if (slot.Amount > 0)
                    return false;
            }

            return true;
        }
    }

    public void Insert(InventoryItem item, int amount=1)
    {
        var slot = Items.FirstOrDefault(i => i.Item == item);
        if (slot != null)
        {
            slot.Amount += amount;
        }
        else
        {
            slot = Items.FirstOrDefault(i => i.Item == null);
            if(slot == null)
            {
                GD.Print("Invetory is full");
                return;
            }
            slot.Item = item;
            slot.Amount = amount;
        }
    }
}
