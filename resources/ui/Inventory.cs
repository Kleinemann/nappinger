using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class Inventory : Resource
{
    [Export] public Array<InventorySlot> Items = new Array<InventorySlot>();

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
