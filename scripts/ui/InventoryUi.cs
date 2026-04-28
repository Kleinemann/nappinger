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

    public void UpdateSlots()
    {
        Inventory inventory = null;

        if (WorldMain.SelectedPlayer != null)
            inventory = WorldMain.SelectedPlayer.Inventory;

        if (WorldMain.SelectedStore != null)
            inventory = WorldMain.SelectedStore.Inventory;

        UpdateSlots(inventory);

        this.Visible = inventory != null;
    }

    public void UpdateSlots(Inventory inventory)
    {
        for (var i = 0; i < Slots.Count; i++)
        {
            if (inventory != null && inventory.Slots.Count > i)
                Slots[i].UpdateSlot(inventory.Slots[i]);
            else
                Slots[i].UpdateSlot(null);
        }
    }
}
