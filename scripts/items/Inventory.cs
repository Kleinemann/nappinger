using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Inventory : Resource
{
    [Export] public Array<InventoryItem> Items = new Array<InventoryItem>();
}
