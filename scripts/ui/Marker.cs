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
    }


    public void Deselect()
    {
        Visible = false;
        CurrentObject = null;
        Ui.Instance.Update();
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
