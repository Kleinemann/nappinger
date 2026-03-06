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
    TextureRect ItemPicture;
    TextureRect ItemIsHuman;

    public ItemClass Item = new ItemClass();

    public override void _Ready()
    {
        Instance = this;

        //Items
        ItemBox = GetNode<VBoxContainer>("ItemBox");
        ItemLabel = GetNode<Label>("ItemBox/ItemName");
        ItemPicture = GetNode<TextureRect>("ItemBox/HBoxContainer/PanelContainer/ItemPicture");
        ItemIsHuman = GetNode<TextureRect>("ItemBox/HBoxContainer/IsHuman");
        DeselectItem();
    }

    public class ItemClass
    {
        public bool Selected { get => Instance.ItemBox.Visible; set => Instance.ItemBox.Visible = value; }
        public string Name { get => Instance.ItemLabel.Text; set => Instance.ItemLabel.Text = value; }
        public Texture2D Picture { get => Instance.ItemPicture.Texture; set => Instance.ItemPicture.Texture = value; }
        public bool IsHuman { get => Instance.ItemIsHuman.Visible; set => Instance.ItemIsHuman.Visible = value; }
    }

    public void SelectItem(Vector2I pos)
    {
        WorldMap Map = WorldMain.Instance.Map;

        TileData data = Map.ItemLayer.GetCellTileData(pos);

        Chunk chunk = WorldMain.Instance.Map.GetChunk(pos);
        GameItem item = chunk.Items[pos];

        Item.Name = item.Name;
        Marker marker = WorldMain.Instance.Map.Marker;
        marker.CurrentItem = item;

        Item.IsHuman = item.Type == ItemType.PLAYER;
        Item.Picture = item.Icon;
        Item.Selected = true;
    }

    public void DeselectItem()
    {
        Item.Name = string.Empty;
        Item.Picture = null;
        Item.IsHuman = false;
        Item.Selected = false;
    }
}