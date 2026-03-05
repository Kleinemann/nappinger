using Godot;
using nappinger.scripts;

public partial class Marker : Sprite2D
{
    public Chunk CurrentChunk = null;
    public GameItem CurrentItem = null;

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
            Position = Map.ItemLayer.MapToLocal(pos);
            Visible = true;
            CurrentChunk = Map.GetChunk(pos);

            Ui.Instance.SelectItem(pos);

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

        Ui.Instance.DeselectItem();
    }

    public void Update()
    {
        if(Visible && CurrentItem != null)
        {
            Vector2I pos = CurrentItem.Position;
            Position = Map.ItemLayer.MapToLocal(pos);
        }
    }
}
