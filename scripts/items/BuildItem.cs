using Godot;
using System.Numerics;

public partial class BuildItem : Area2D
{
    DropItem Drop;
    public Vector2I AtlasCoords;
    public Vector2I WorldCoords;

    public override void _Ready()
    {
        Drop = GetNode<DropItem>("DropItem");
        CollisionShape2D collision = GetNode<CollisionShape2D>("CollisionShape2D");
        BodyEntered += BuildItem_BodyEntered;

        CollisionShape2D dropCollision = Drop.GetNode<CollisionShape2D>("CollisionShape2D");
        dropCollision.Disabled = true;
    }

    private void BuildItem_BodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            Player player = (Player)body;
            string groupName = Drop.Item.GroupName;
            int costs = Drop.Amount;

            int rest = player.Inventory.Remove(Drop.Item, costs);
            if (rest == 0)
            {
                _ = player.DoWork(this);
            }
        }
    }

    public static BuildItem CreateBuildItem(Button btnBuild, Vector2I wordCoords)
    {
        Vector2I atlasCoords = (Vector2I)btnBuild.GetMeta("Atlas");

        if(atlasCoords == new Vector2I(-1, -1))
            return null;

        PackedScene scene = GD.Load<PackedScene>("res://szenes/objects/BuildItem.tscn");
        BuildItem item = scene.Instantiate<BuildItem>();
        item.AtlasCoords = atlasCoords;
        item.WorldCoords = wordCoords;
        return item;
    }
}
