using Godot;

[GlobalClass]
public partial class ItemBase : Resource
{
    const int MaxCount = 10;

    [Export] public int ID { get; set; }
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Texture { get; set; }
    [Export] public int Count { get; set; }

    public int Add(int value)
    {
        if(Count + value <= MaxCount)
        {
            Count += value;
            return 0;
        }

        int ret = MaxCount - Count - value;
        Count = MaxCount;
        return ret;
    }
}
