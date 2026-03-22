using Godot;
using Godot.Collections;

public partial class ItemBase : Sprite2D
{
    Label ItemValue;
    public int ID { get; set; }
    public string Namen { get; set; }

    int _value = 0;
    public int Value 
    { 
        get {  return _value; }
        set
        {
            _value = value;
            ItemValue.Text = _value.ToString();
        }
    }

    public static ItemBase NewItem(int id, int value = 1)
    {
        PackedScene scene = GD.Load("res://szenes/items/ItemBase.tscn") as PackedScene;
        ItemBase item = scene.Instantiate<ItemBase>();
        item.ItemValue = item.GetNode<Label>("ItemValue");

        ItemData data = new ItemData(id);
        item.ID = id;
        item.Name = data.Name;
        item.Texture = GD.Load(data.TexturePath) as Texture2D;
        item.Value = value;

        return item;
    }

    internal class ItemData
    {
        public int ID;
        public string Name;
        public string TexturePath;

        public ItemData(int id)
        {
            ID = id;

            switch (id)
            {
                case 1:
                    Name = "Holz";
                    TexturePath = "res://assets/items/wood.png";
                    break;

                case 2:
                    Name = "Essen";
                    TexturePath = "res://assets/items/food.png";
                    break;

                default:
                    Name = "Empty";
                    TexturePath = "res://assets/items/empty.png";
                    break;
            }
        }
    }

}
