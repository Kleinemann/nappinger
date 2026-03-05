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

        Item.Name = (string)data.GetCustomData("ItemName");
        ItemType ItemType = (ItemType)((int)data.GetCustomData("ItemType"));
        int ItemValue = (int)data.GetCustomData("ItemValue");

        Marker marker = WorldMain.Instance.Map.Marker;
        if (ItemType == ItemType.ANIMAL)
        {
            int key = marker.CurrentChunk.Animals.First(x => x.Value == pos).Key;
            marker.CurrentAnimal = key;
        }
        else marker.CurrentAnimal = -1;

        if (ItemType == ItemType.PLAYER)
        {
            int key = marker.CurrentChunk.Player.First(x => x.Value == pos).Key;
            marker.CuttentPlayer = key;
            Item.IsHuman = true;
        }
        else
        {
            marker.CuttentPlayer = -1;
            Item.IsHuman = false;
        }

        int sourceId = Map.ItemLayer.GetCellSourceId(pos);
        Vector2I atlasCoords = Map.ItemLayer.GetCellAtlasCoords(pos);
        TileSetAtlasSource tss = Map.ItemLayer.TileSet.GetSource(sourceId) as TileSetAtlasSource;
        Texture2D atlasTexture = tss.Texture;
        Rect2I region = tss.GetTileTextureRegion(atlasCoords);

        Image atlasImage = atlasTexture.GetImage();
        Image tileImage = atlasImage.GetRegion(region);

        Item.Picture = ImageTexture.CreateFromImage(tileImage);

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