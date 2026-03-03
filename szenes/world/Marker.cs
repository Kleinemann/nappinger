using Godot;
using System;
using System.Linq;
using static WorldMain;

public partial class Marker : Sprite2D
{
    public Chunk CurrentChunk = null;
    public ItemType ItemType = ItemType.NONE;
    public string ItemName;
    public int ItemValue;

    public int CurrentAnimal = -1;

    WorldMap Map;

    public override void _Ready()
    {
        Visible = false;
        Map = GetParent<WorldMap>();
    }

    public bool Select(Vector2I pos)
    {
        int atlas = Map.ItemLayer.GetCellSourceId(pos);
        if (atlas >= 0)
        {
            TileData data = Map.ItemLayer.GetCellTileData(pos);

            ItemName = (string)data.GetCustomData("ItemName");
            ItemType = (ItemType)((int)data.GetCustomData("ItemType"));
            ItemValue = (int)data.GetCustomData("ItemValue");

            Position = Map.ItemLayer.MapToLocal(pos);
            Visible = true;
            CurrentChunk = Map.GetChunk(pos);

            if(ItemType == ItemType.ANIMAL)
            {
                int key = CurrentChunk.Animals.First(x => x.Value == pos).Key;
                CurrentAnimal = key;
            }
            else CurrentAnimal = -1;

            GD.Print("Clicked on: " + ItemName);
            return true;
        }
        else
            Deselect();
        return false;
    }


    public void Deselect()
    {
        Visible = false;
        CurrentChunk = null;
        ItemType = ItemType.NONE;
        ItemName = "";
        ItemValue = 0;

        CurrentAnimal = -1;
    }

    public void Update()
    {
        if(Visible && CurrentAnimal >= 0)
        {
            Vector2I pos = CurrentChunk.Animals[CurrentAnimal];
            Position = Map.ItemLayer.MapToLocal(pos);
        }
    }
}
