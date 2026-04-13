using Godot;
using System;

[GlobalClass]
public partial class InventorySlot : Resource
{
    [Export] public InventoryItem Item;
    [Export] public int Amount;
}
