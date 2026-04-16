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

    public void UpdatePanel()
    {
        this.Visible = WorldMain.SelectedObject != null;

        if (WorldMain.SelectedObject is Animal animal)
        {
            ObjectName.Text = animal.Name;
            Icon.Texture = animal.Icon;
            HealtBar.Show();
            HealtBar.Value = animal.Healt;
            HealtBar.MaxValue = animal.MaxHealt;
        }
        else if(WorldMain.SelectedObject is BreakableObject obj)
        {
            ObjectName.Text = obj.ObjectName;
            Icon.Texture = obj.Icon;
            HealtBar.Show();
            HealtBar.Value = obj.Healt;
            HealtBar.MaxValue = obj.MaxHealt;
        }
        else if(WorldMain.SelectedObject is Store store)
        {
            ObjectName.Text = store.ObjectName;
            Icon.Texture = store.Icon;
            HealtBar.Hide();
            HealtBar.Value = 1;
            HealtBar.MaxValue = 1;
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
