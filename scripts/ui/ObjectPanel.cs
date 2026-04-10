using Godot;
using System;
using System.Security.AccessControl;

public partial class ObjectPanel : Panel
{
    Label ObjectName;
    TextureRect Icon;
    ProgressBar HealtBar;

    public override void _Ready()
    {
        ObjectName = GetNode<Label>("ObjectName");
        Icon = GetNode<TextureRect>("Icon");
        HealtBar = GetNode<ProgressBar>("HealtBar");
    }

    public void UpdatePanel(object selection)
    {
        if (selection is Player player)
        {
            ObjectName.Text = player.Name;
            Icon.Texture = player.Icon;
            HealtBar.Value = player.Healt;
            HealtBar.MaxValue = player.MaxHealt;
        }
        else if(selection is BreakableObject obj)
        {
            ObjectName.Text = obj.ObjectName;
            Icon.Texture = obj.Icon;
            HealtBar.Value = obj.Healt;
            HealtBar.MaxValue = obj.MaxHealt;
        }
        else
        {
            ObjectName.Text = "";
            Icon.Texture = null;
            HealtBar.Value = 1;
            HealtBar.MaxValue = 1;
        }
    }
}
