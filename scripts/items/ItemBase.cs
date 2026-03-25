using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemBase : Resource
{
    public const int MaxCount = 99;

    [Export] public int ID { get; set; }
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Texture { get; set; }
    [Export] public int Count { get; set; }

    public int Add(int value)
    {
        int ret;
        if (Count + value <= MaxCount)
        {
            Count += value;
            ret = 0;
        }
        else
        {
            ret = MaxCount - Count - value;
            Count = MaxCount;
        }

        return ret;
    }

    public int Remove(int value)
    {
        int ret;
        if (Count >= value)
        {
            Count -= value;
            ret = 0;
        }
        else
            ret = -value;

        return -value;
    }



    #region Item Lists

    public static ItemBase GetItem(int ID)
    {
        return (ItemBase)ItemList[ID].Duplicate();
    }


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
    #endregion
}
