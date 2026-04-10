using Godot;
using System;
using System.Security.AccessControl;

public partial class ObjectPanel : Panel
{
    Label ObjectName;
    ProgressBar HealtBar;

    public override void _Ready()
    {
        ObjectName = GetNode<Label>("ObjectName");
        HealtBar = GetNode<ProgressBar>("HealtBar");
    }

    public void Update(object selection)
    {
        if (selection is Player player)
        {
            ObjectName.Text = player.Name;
            HealtBar.Value = player.Healt;
            HealtBar.MaxValue = player.MaxHealt;
        }
        else if(selection is BreakableObject obj)
        {
            ObjectName.Text = obj.ObjectName;
            HealtBar.Value = obj.Healt;
            HealtBar.MaxValue = obj.MaxHealt;
        }
        else
        {
            ObjectName.Text = "";
            HealtBar.Value = 1;
            HealtBar.MaxValue = 1;
        }
    }
}
