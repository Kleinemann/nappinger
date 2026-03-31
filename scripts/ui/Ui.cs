using Godot;
using static WorldMain;

public partial class Ui : Control
{
    public static Ui Instance;

    // Items
    public ObjectOverview ObjectInfo;
    public ActionBar ActionBar;

    public override void _Ready()
    {
        Instance = this;

        ObjectInfo = GetNode<ObjectOverview>("ObjectOverview");
        ActionBar = GetNode<ActionBar>("ActionBar");
    }

    public void Update()
    {
        ObjectInfo.Update();
    }
}