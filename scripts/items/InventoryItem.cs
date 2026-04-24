using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InventoryItem : Resource
{
    [Export] public string ItemName = "";
    [Export] public string GroupName;
    [Export] public string Description = "";
    [Export] public Texture2D Icon;

    public static InventoryItem CreateInventoryItem(string resourceName)
    {
        if(resourceName.StartsWith("R_"))
            resourceName = resourceName.Substring(2);

        return GD.Load<Resource>($"res://resources/items/{resourceName.ToLower()}.tres") as InventoryItem;
    }
}
