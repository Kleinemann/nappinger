using Godot;
using Godot.Collections;
using System.Linq;

public partial class InventoryUi : Control
{
    Array<InventorySlot> Slots = new Array<InventorySlot>();

    public override void _Ready()
    {
        GridContainer grid = GetNode<GridContainer>("GridContainer");
        var children = grid.GetChildren();
        foreach (var child in children)
        {
            if (child is InventorySlot slot)
            {
                Slots.Add(slot);
            }
        }
    }

    public void UpdateSlots(Inventory inventory)
    {
        for(var i =0; i < Slots.Count; i++)
        {
            if(inventory != null)
                Slots[i].UpdateSlot(inventory.Items[i]);
            else
                Slots[i].UpdateSlot(null);
        }
    }
}
