using Godot;
using nappinger.scripts;

public partial class Marker : Sprite2D
{
    public GameItem CurrentItem = null;
    WorldMap Map;

    public override void _Ready()
    {
        Visible = false;
        Map = GetParent<WorldMap>();
    }

    public void Select(GameItem item)
    {
        Visible = true;
        CurrentItem = item;
        Update();
        Ui.Instance.Update();
        //Input.SetCustomMouseCursor(null);
        //int atlas = Map.ItemLayer.GetCellSourceId(pos);
        //if (atlas >= 0)
        //{
        //    Position = Map.ItemLayer.MapToLocal(pos);
        //    Visible = true;

        //    Ui.Instance.SelectItem(pos);

        //    return true;
        //}
        //else
        //    Deselect();
        //return false;
    }


    public void Deselect()
    {
        Visible = false;
        CurrentItem = null;
        Ui.Instance.Update();
        //Input.SetCustomMouseCursor(null);
        //Ui.Instance.DeselectItem();
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
