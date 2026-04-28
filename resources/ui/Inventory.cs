using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class Inventory : Resource
{
    [Export] public Array<InventorySlot> Slots = new Array<InventorySlot>();
    [Export] public int SlotsMax = 10;
    [Export] public int MaxStackSize = 10;

    public Inventory()
    {
        for (var i = 0; i < SlotsMax; i++)
        {
            InventorySlot slot = new InventorySlot() { ResourceLocalToScene = true }; 
            Slots.Add(slot);
        }
    }

    public int CountItemGroup(string group)
    {
        int amountSum = 0;
        foreach (var slot in Slots)
        {
            if(slot.Item != null && slot.Item.GroupName == group)
                amountSum += slot.Amount;
        }
        return amountSum;
    }

    public bool IsEmpty
    {
        get
        {
            foreach (var slot in Slots)
            {
                if (slot.Amount > 0)
                    return false;
            }

            return true;
        }
    }

    public int Insert(InventoryItem item, int amount=1)
    {
        bool inveoryFull = false;
        while (!inveoryFull && amount > 0)
        {
            InventorySlot slot = First(item);
            if(slot == null)
            {
                inveoryFull = true;
            }
            else
            {
                slot.Amount += amount;
                amount = 0;

                if (slot.Amount > MaxStackSize)
                {
                    amount = slot.Amount - MaxStackSize;
                    slot.Amount = MaxStackSize;
                }
            }
        }

        return amount;
    }

    InventorySlot First(InventoryItem item)
    {
        InventorySlot empty = null;
        foreach (InventorySlot slot in Slots)
        {
            if(empty == null && slot.Item == null)
                empty = slot;

            if(slot != null)
            {
                if (slot.Item == item && slot.Amount < MaxStackSize)
                    return slot;
            }
        }

        if(empty != null)
            empty.Item = item;
        return empty;
    }
}
