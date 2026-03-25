using Godot;
using System;

public partial class DropItem : Area2D
{
    [Export]
    public ItemBase Item;

    public static DropItem NewItem(int id, int count = 1)
    {
        PackedScene ps = GD.Load<PackedScene>("res://szenes/gameObjects/DropItem.tscn");
        DropItem di = ps.Instantiate<DropItem>();

        di.Item = ItemBase.GetItem(id);
        di.Item.Count = count;

        Sprite2D sprite = di.GetNode<Sprite2D>("Sprite2D");
        sprite.Texture = di.Item.Texture;

        return di;
    }

    public static void DropNewItem(Vector2I coord, int id, int count = 1)
    {
        DropItem di = NewItem(id, count);

        WorldMap map = WorldMain.Instance.Map;
        Chunk chunk = map.GetChunk(coord);

        foreach(Vector2I c in chunk.GetNeigbours(coord))
        {
            if(chunk.GetItem(c) == null)
            {
                if (chunk.Drops.ContainsKey(c) && chunk.Drops[coord].Item.ID == id)
                {
                    chunk.Drops[coord].Item.Count += count;
                }
                else if(!chunk.Drops.ContainsKey(coord))
                {
                    di.Position = map.CoordsToPosition(coord);
                    chunk.Drops.Add(coord, di);
                    map.AddChild(di);
                }
            }
        }
    }


    public override void _Ready()
    {
        if (Item == null)
        {
            Item = GD.Load<ItemBase>("res://resources/items/Wood.tres");
            Item.Count = 1;
        }

        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");

        sprite.Texture = Item.Texture; 
    }
}
