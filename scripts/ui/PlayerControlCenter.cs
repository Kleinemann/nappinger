using Godot;
using System;
using System.Linq;

public partial class PlayerControlCenter : Control
{
    GridContainer Grid;
    CraftingButton BtnWorker;
    CraftingButton BtnMetal;

    public override void _Ready()
    {
        Grid = GetNode<GridContainer>("Panel/GridContainer");

        BtnWorker = GetNode<CraftingButton>("Panel/HBoxContainer/BtnWorker");
        BtnWorker.Pressed += BtnWorker_Pressed;

        BtnMetal = GetNode<CraftingButton>("Panel/HBoxContainer/BtnMetal");
        BtnMetal.Pressed += BtnMetal_Pressed;

        Button btnClose = GetNode<Button>("Panel/ButtonClose");
        btnClose.Pressed += BtnClose_Pressed;
    }

    private void BtnMetal_Pressed()
    {
        BtnMetal.Pay();
        InventoryItem item = InventoryItem.CreateInventoryItem("metal");
        BtnMetal.Inv.Insert(item);
    }

    private void BtnClose_Pressed()
    {
        HidePlayers();
    }

    private void BtnWorker_Pressed()
    {
        //remove materials
        BtnWorker.Pay();

        //Creat Player
        PackedScene scene = GD.Load<PackedScene>("res://szenes/character/Player.tscn");
        Player player = scene.Instantiate<Player>();

        Store store = WorldMain.SelectedObject as Store;
        player.Position = store.Position + new Vector2(20, 100);
        player.ObjectName = Player.GetRandomName();

        WorldMain.Instance.Map.AddChild(player);

        ReloadPlayers();
    }

    void ReloadPlayers()
    {
        HidePlayers();
        ShowPlayers();
    }

    public void ShowPlayers()
    {
        this.Show();
        var players = WorldMain.Instance.Map.GetChildren().Where(x => x is Player).ToArray();
        foreach (Player player in players)
        {
            PackedScene scene = GD.Load<PackedScene>("res://szenes/ui/player_control_item.tscn");
            PlayerControlItem item = scene.Instantiate<PlayerControlItem>();
            player.State = GameObjectState.IDLE;
            player.Mission = null;
            item.SetPlayer(player);
            Grid.AddChild(item);
        }

        BtnWorker.Inv = WorldMain.SelectedStore.Inventory;
        BtnMetal.Inv = WorldMain.SelectedStore.Inventory;
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

    public void UpdateControlCenter()
    {
        foreach(PlayerControlItem player in Grid.GetChildren())
        {
            player.UpdatePlayerItem();
        }

        BtnWorker.UpdateButton();
        BtnMetal.UpdateButton();
    }
}
