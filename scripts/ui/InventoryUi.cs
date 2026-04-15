using Godot;
using Godot.Collections;
using System.Linq;

public partial class InventoryUi : Control
{
    Array<InventorySlotUI> Slots = new Array<InventorySlotUI>();

    public override void _Ready()
    {
        GridContainer grid = GetNode<GridContainer>("GridContainer");
        var children = grid.GetChildren();
        foreach (var child in children)
        {
            if (child is InventorySlotUI slot)
            {
                Slots.Add(slot);
            }
        }
    }

    public void UpdateSlots(Inventory inventory)
    {
        for(var i =0; i < Slots.Count; i++)
        {
            if(inventory != null && inventory.Items.Count > i)
                Slots[i].UpdateSlot(inventory.Items[i]);
            else
                Slots[i].UpdateSlot(null);
        }
    }
}
