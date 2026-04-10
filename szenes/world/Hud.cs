using Godot;
using System;

public partial class Hud : Control
{
    ObjectPanel ObjectPanel;
    InventoryUi InventoryUi;

    public static Hud Instance;
    
    public override void _Ready()
    {
        Instance = this;
        ObjectPanel = GetNode<ObjectPanel>("ObjectPanel");
        InventoryUi = GetNode<InventoryUi>("InventoryUI");
        UpdateHud();
    }

    public void UpdateHud()
    {
        object selection = Player.SelectetPlayer != null ? Player.SelectetPlayer 
                    : BreakableObject.SelectedObject != null ? BreakableObject.SelectedObject : null;

        ObjectPanel.Visible = selection != null;

        if (selection != null)
        {
            ObjectPanel.Update(selection);
            InventoryUi.Update(selection);
        }
    }
}
