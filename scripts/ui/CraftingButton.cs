using Godot;
using Godot.Collections;
using System;

public partial class CraftingButton : Button
{
    [Export] public Array<InventorySlot> Items = new Array<InventorySlot>();
    public Inventory Inv;

    public void UpdateButton()
    {
        bool enabled = true;
        if (Inv != null)
        {
            foreach (InventorySlot slot in Items)
            {
                if(Inv.CountItemGroup(slot.Item.GroupName) < slot.Amount)
                {
                    enabled = false;
                    break;
                }
            }
        }
        else
            enabled = false;

        Disabled = !enabled;
    }

    internal void Pay()
    {
        foreach (InventorySlot slot in Items)
        {
            InventoryItem item = slot.Item;
            int cost = slot.Amount;

            for(int s = Inv.SlotsMax-1; s >= 0; s--)
            {
                InventorySlot invS = Inv.Slots[s];
                if (invS.Item == item)
                {
                    //mehr als nötig
                    if(cost < invS.Amount)
                    {
                        invS.Amount -= cost;
                    }
                    else
                    {
                        cost -= invS.Amount;
                        invS.Amount = 0;
                        invS.Item = null;
                    }
                }                
            }
        }
    }
}
