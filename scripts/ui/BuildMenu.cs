using Godot;
using System;

public partial class BuildMenu : Panel
{
    Button btn1;
    Button btn2;
    Button btn3;

    public static Button SelectedButton;
    public static BuildMenu Instance;

    public override void _Ready()
    {
        btn1 = GetNode<Button>("HFlowContainer/BtnFloor1");
        btn2 = GetNode<Button>("HFlowContainer/BtnFloor2");
        btn3 = GetNode<Button>("HFlowContainer/BtnFloorDel");

        btn1.Pressed += () => Btn_Pressed(btn1);
        btn2.Pressed += () => Btn_Pressed(btn2);
        btn3.Pressed += () => Btn_Pressed(btn3);

        SelectedButton = btn1;

        Hidden += BuildMenu_Hidden;
    }

    private void BuildMenu_Hidden()
    {
        SelectedButton = null;
    }

    private void Btn_Pressed(Button button)
    {
        SelectedButton = button;
    }

    public void ShowBuildMenu()
    {
        Instance = this;        
        Show();
    }

    public void HideBuildMenu()
    {
        Instance = null;
        Hide();
    }

    public void CreateBuildItem()
    {
        WorldMap map = WorldMain.Instance.Map;

        Vector2 gMouse = GetGlobalMousePosition() * WorldMain.Instance.Camera.Scale;
        Vector2I mouseCoords = map.BuildingFloor.LocalToMap(gMouse);


        if (map.BuildingFloor.GetCellAtlasCoords(mouseCoords) == new Vector2I(-1, -1))
        {
            map.BuildingFloor.SetCell(mouseCoords, 1, new Vector2I(0, 0), 0); 
        }
        //Vector2I coords = WorldMain.Instance.Map.GetMouseCoords() * Chunk.TileSize;

        //coords.X -= coords.X % (Chunk.TileSize * 2);
        //coords.Y -= coords.Y % (Chunk.TileSize * 2);

        //GD.Print(coords);

        //WorldMap map = WorldMain.Instance.Map;
        //Chunk chunk = map.GetChunk(WorldMain.Instance.Map.GetMouseCoords());

        //BuildItem item = BuildItem.CreateBuildItem(SelectedButton);

        //foreach (BuildItem bi in chunk.BuildItems)
        //{
        //    if (bi.Position == coords)
        //    {
        //        if (item == null)
        //        {
        //            chunk.BuildItems.Remove(bi);
        //            bi.QueueFree();
        //        }
        //        return;
        //    }
        //}

        //if(item == null)
        //    return;

        //item.Position = coords;
        //WorldMain.Instance.AddChild(item);
        //chunk.BuildItems.Add(item);
    }
}
