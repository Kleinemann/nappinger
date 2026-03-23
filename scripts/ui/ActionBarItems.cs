using Godot;
using Godot.Collections;


public partial class ActionBarItems : Resource
{
    [Export] public Array<ItemBase> Items { get; set; }

    static Dictionary<int, ItemBase> _itemList;

    public static Dictionary<int, ItemBase> ItemList
    {
        get
        {
            if (_itemList == null)
            {
                _itemList = new Dictionary<int, ItemBase>();

                string path = "res://resources/items/";
                foreach (string file in DirAccess.GetFilesAt(path))
                {
                    ItemBase item = GD.Load<ItemBase>(path + file);
                    _itemList.Add(item.ID, item);
                }
            }
            return _itemList;
        }
    }

    public ItemBase GetItem(int ID)
    {
        return ItemList[ID];
    }
}