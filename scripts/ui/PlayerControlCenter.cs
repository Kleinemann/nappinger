using Godot;
using System;
using System.Linq;

public partial class PlayerControlCenter : Control
{
    GridContainer Grid;

    public override void _Ready()
    {
        Grid = GetNode<GridContainer>("Panel/GridContainer");
    }

    public void ShowPlayers()
    {
        this.Show();
        var players = WorldMain.Instance.Map.GetChildren().Where(x => x is Player).ToArray();
        foreach (Player player in players)
        {
            PackedScene scene = GD.Load<PackedScene>("res://szenes/ui/player_control_item.tscn");
            PlayerControlItem item = scene.Instantiate<PlayerControlItem>();
            item.SetPlayer(player);
            Grid.AddChild(item);
        }
    }

    public void HidePlayers()
    {
        for (int i = Grid.GetChildCount() - 1; i >= 0; i--)
        {
            Node Child = Grid.GetChild(i);
            if(Child != null)
                Child.QueueFree();

        }
        this.Hide();
    }
}
