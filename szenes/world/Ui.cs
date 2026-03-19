using Godot;
using nappinger.scripts;
using System;
using System.Linq;
using static WorldMain;

public partial class Ui : Control
{
    public static Ui Instance;

    // Items
    VBoxContainer ItemBox;
    Label ItemLabel;
    Label ItemValue;
    TextureRect ItemPicture;
    TextureRect ItemIsHuman;
    Label ItemCoord;

    public override void _Ready()
    {
        Instance = this;

        //Items
        ItemBox = GetNode<VBoxContainer>("ItemBox");
        ItemLabel = GetNode<Label>("ItemBox/HBoxContainer2/ItemName");
        ItemValue = GetNode<Label>("ItemBox/HBoxContainer2/ItemValue");
        ItemPicture = GetNode<TextureRect>("ItemBox/HBoxContainer/PanelContainer/ItemPicture");
        ItemIsHuman = GetNode<TextureRect>("ItemBox/HBoxContainer/IsHuman");
        ItemCoord = GetNode<Label>("ItemBox/HBoxContainer/ItemCoord");
        Visible = false;
    }

    public void Update()
    {
        GameItem item = WorldMain.Instance.Map.Marker.CurrentItem;
        if(item != null)
        {
            Visible = true;
            ItemLabel.Text = item.ItemName;
            ItemValue.Text = item.Value.ToString();
            ItemPicture.Texture = item.Icon;
            ItemIsHuman.Visible = item.ItemType == ItemTypeEnum.PLAYER;
            ItemCoord.Text = $" ( {item.Position.X} | {item.Position.Y} )";
        }
        else
        {
            Visible = false;
        }
    }
}