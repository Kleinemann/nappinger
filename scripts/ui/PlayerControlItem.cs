using Godot;
using System;

public partial class PlayerControlItem : Control
{
    Player Player;

    TextureRect Icon;
    Label PlayerName;
    ButtonGroup Group;


    public override void _Ready()
    {
        Icon = GetNode<TextureRect>("PanelContainer/HBoxContainer/Icon");
        PlayerName = GetNode<Label>("PanelContainer/HBoxContainer/Label");
        Button b = GetNode<Button>("PanelContainer/HBoxContainer/BtnIdle");
        Group = b.ButtonGroup;

        Icon.Texture = Player.Icon;
        PlayerName.Text = Player.ObjectName;

        Group.Pressed += Group_Pressed;
    }

    private void Group_Pressed(BaseButton button)
    {
        if (button.Name == "BtnIdle")
            Player.SetSearch(null);
        else
        {
            string search = ((string)button.Name).Substring(3);
            Player.SetSearch("R_" + search);
        }
    }

    internal void SetPlayer(Player player)
    {
        Player = player;
    }
}
