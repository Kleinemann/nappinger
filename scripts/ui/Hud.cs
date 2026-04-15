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
        object selection = Animal.SelectetAnimal != null ? Animal.SelectetAnimal 
                    : BreakableObject.SelectedObject != null ? BreakableObject.SelectedObject : null;

        ObjectPanel.Visible = selection != null;

        if (selection != null)
        {
            ObjectPanel.UpdatePanel(selection);
        }

        if (Animal.SelectetAnimal != null)
        {
            InventoryUi.Show();
            InventoryUi.UpdateSlots(Animal.SelectetAnimal.Inventory);
        }
        else
        {
            InventoryUi.UpdateSlots(null);
            InventoryUi.Hide();
        }
    }
}
