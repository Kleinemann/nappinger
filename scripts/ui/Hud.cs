using Godot;
using System;

public partial class Hud : Control
{
    ObjectPanel ObjectPanel;
    InventoryUi InventoryUi;
    PlayerControlCenter PlayerControlCenter;
    BuildMenu BuildMenu;

    public Button BtnCamp;
    public Button BtnBuild;
    public Button BtnPlant;

    public static Hud Instance;
    
    public override void _Ready()
    {
        Instance = this;
        ObjectPanel = GetNode<ObjectPanel>("ObjectPanel");
        InventoryUi = GetNode<InventoryUi>("InventoryUI");
        PlayerControlCenter = GetNode<PlayerControlCenter>("PlayerControlCenter");
        BuildMenu = GetNode<BuildMenu>("BuildMenu");

        BtnCamp = GetNode<Button>("GridContainer/BtnCamp");
        BtnBuild = GetNode<Button>("GridContainer/BtnBuild");
        BtnPlant = GetNode<Button>("GridContainer/BtnPlant");

        BtnCamp.Toggled += BtnCamp_Toggled;
        BtnBuild.Toggled += BtnBuild_Toggled;
        BtnPlant.Toggled += BtnPlant_Toggled;

        UpdateHud();
    }

    private void BtnBuild_Toggled(bool toggledOn)
    {
        if (toggledOn)
        {
            BuildMenu.ShowBuildMenu();
        }
        else
            BuildMenu.HideBuildMenu();
    }

    private void BtnCamp_Toggled(bool toggledOn)
    {
        if (toggledOn)
        {
            var nodes = GetTree().GetNodesInGroup("Storable");
            //TODO: dound the nearest
            if (nodes.Count > 0)
                WorldMain.SelectedObject = (Node2D)nodes[0];

            PlayerControlCenter.ShowPlayers();
        }
        else
            PlayerControlCenter.HidePlayers();
    }

    private void BtnPlant_Toggled(bool toggledOn)
    {
        if (toggledOn)
        {

        }
        else
        {
        }
    }

    public void UpdateHud()
    {
        ObjectPanel.UpdatePanel();
        InventoryUi.UpdateSlots();
        if (PlayerControlCenter.Visible)
            PlayerControlCenter.UpdateControlCenter();
    }
}
