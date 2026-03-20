using Godot;
using nappinger.scripts;

public partial class Marker : Sprite2D
{
    public GameObject CurrentObject = null;
    WorldMap Map;

    public override void _Ready()
    {
        Visible = false;
        Map = GetParent<WorldMap>();
    }

    public void Select(GameObject item)
    {
        Visible = true;
        CurrentObject = item;
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
        CurrentObject = null;
        Ui.Instance.Update();
        //Input.SetCustomMouseCursor(null);
        //Ui.Instance.DeselectItem();
    }

    public void Update()
    {
        if(Visible && CurrentObject != null)
        {
            Vector2I pos = CurrentObject.Position;
            Position = Map.ObjectLayer.MapToLocal(pos);
        }
    }
}
