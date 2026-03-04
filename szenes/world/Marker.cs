using Godot;

public partial class Marker : Sprite2D
{
    public Chunk CurrentChunk = null;
    public int CurrentAnimal = -1;
    public int CuttentPlayer = -1;

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
        if(Visible && CurrentAnimal >= 0)
        {
            Vector2I pos = CurrentChunk.Animals[CurrentAnimal];
            Position = Map.ItemLayer.MapToLocal(pos);
        }

        if (Visible && CuttentPlayer >= 0)
        {
            Vector2I pos = CurrentChunk.Player[CuttentPlayer];
            Position = Map.ItemLayer.MapToLocal(pos);
        }
    }
}
