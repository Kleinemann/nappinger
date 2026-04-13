using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Inventory : Resource
{
    [Export] public Array<InventorySlot> Items = new Array<InventorySlot>();
}
