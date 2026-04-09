using Godot;
using System;

public partial class Hud : Control
{
    Label ObjectName;
    ProgressBar HealtBar;
    public static Hud Instance;
    public override void _Ready()
    {
        Instance = this;
        ObjectName = GetNode<Label>("ObjectName");
        HealtBar = GetNode<ProgressBar>("HealtBar");
        UpdateHud();
    }

    public void UpdateHud()
    {
        Player player = Player.SelectetPlayer;

        if (player == null)
        {
            ObjectName.Text = "";
            HealtBar.Value = 1;
            HealtBar.MaxValue = 1;
            return;
        }
        ObjectName.Text = player.ObjectName;
        HealtBar.Value = player.Healt;
        HealtBar.MaxValue = player.MaxHealt;
    }
}
