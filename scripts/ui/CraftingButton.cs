using Godot;
using Godot.Collections;

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
                if(Inv.CountItemGroup(slot.Item.GroupName) <= slot.Amount)
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
}
