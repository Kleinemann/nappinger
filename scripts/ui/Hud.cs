using Godot;
using System;

public partial class Hud : Control
{
    ObjectPanel ObjectPanel;
    InventoryUi InventoryUi;
    PlayerControlCenter PlayerControlCenter;
    BuildMenu BuildMenu;

    public static Hud Instance;
    
    public override void _Ready()
    {
        Instance = this;
        ObjectPanel = GetNode<ObjectPanel>("ObjectPanel");
        InventoryUi = GetNode<InventoryUi>("InventoryUI");
        PlayerControlCenter = GetNode<PlayerControlCenter>("PlayerControlCenter");
        BuildMenu = GetNode<BuildMenu>("BuildMenu");
        UpdateHud();
    }

    public void UpdateHud()
    {
        ObjectPanel.UpdatePanel();
        InventoryUi.UpdateSlots();
        if (PlayerControlCenter.Visible)
            PlayerControlCenter.UpdateControlCenter();
    }

    public void SwitchPlayerControlCenter()
    {
        if (!PlayerControlCenter.Visible)
            PlayerControlCenter.ShowPlayers();
        else
            PlayerControlCenter.HidePlayers();
    }

    public void SwitchBuildMenu()
    {
        if (!BuildMenu.Visible)
            BuildMenu.ShowBuildMenu();
        else
            BuildMenu.HideBuildMenu();
    }
}
