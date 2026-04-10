using Godot;
using System;

[GlobalClass]
public partial class InventoryItem : Resource
{
    [Export] public string ItemName = "";
    [Export] public string Description = "";
    [Export] public Texture2D Icon;
}
