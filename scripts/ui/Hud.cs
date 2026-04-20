using Godot;
using System;

public partial class Hud : Control
{
    ObjectPanel ObjectPanel;
    InventoryUi InventoryUi;
    PlayerControlCenter PlayerControlCenter;

    public static Hud Instance;
    
    public override void _Ready()
    {
        Instance = this;
        ObjectPanel = GetNode<ObjectPanel>("ObjectPanel");
        InventoryUi = GetNode<InventoryUi>("InventoryUI");
        PlayerControlCenter = GetNode<PlayerControlCenter>("PlayerControlCenter");
        UpdateHud();
    }

    public void UpdateHud()
    {
        ObjectPanel.UpdatePanel();
        InventoryUi.UpdateSlots();
    }

    public void SwitchPlayerControlCenter()
    {
        if (!PlayerControlCenter.Visible)
            PlayerControlCenter.ShowPlayers();
        else
            PlayerControlCenter.HidePlayers();
    }
}
