using Godot;
using System;

public partial class PlayerControlItem : Control
{
    Player Player;

    TextureRect Icon;
    Label PlayerName;
    ButtonGroup Group;
    InventoryUi InvUi;

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>("PanelContainer/HBoxContainer/Icon");
        PlayerName = GetNode<Label>("PanelContainer/HBoxContainer/Label");
        Button b = GetNode<Button>("PanelContainer/HBoxContainer/BtnIdle");
        Group = b.ButtonGroup;

        InvUi = GetNode<InventoryUi>("InventoryUI");

        Icon.Texture = Player.Icon;
        PlayerName.Text = Player.ObjectName;
        if(Player.Mission != null && Player.Mission.State == GameObjectState.FARMING)
        {
            string name = ((string)Player.Mission.Target).Substring(2);
            Button btn = GetNode<Button>("PanelContainer/HBoxContainer/Btn" + name);
            btn.ButtonPressed = true;
        }
        Group.Pressed += Group_Pressed;
    }


    private void Group_Pressed(BaseButton button)
    {
        if (button.Name == "BtnIdle")
        {
            Player.Mission = null;
            Player.State = GameObjectState.IDLE;
        }
        else
        {
            string search = "R_" + ((string)button.Name).Substring(3);
            Player.Mission = new Mission(GameObjectState.FARMING, search);
            Player.State = GameObjectState.IDLE;
        }
    }

    internal void SetPlayer(Player player)
    {
        Player = player;
    }

    public void UpdatePlayerItem()
    {
        InvUi.UpdateSlots(Player.Inventory);
    }
}
