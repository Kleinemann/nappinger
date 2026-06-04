using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class BuildMenu : Panel
{
    Button btn1;
    Button btn2;
    Button btn3;
    Button btn4;

    public static Button SelectedButton;
    public static BuildMenu Instance;

    public override void _Ready()
    {
        btn1 = GetNode<Button>("HFlowContainer/BtnFloor1");
        btn2 = GetNode<Button>("HFlowContainer/BtnFloor2");
        btn3 = GetNode<Button>("HFlowContainer/BtnFloorDel");
        btn4 = GetNode<Button>("HFlowContainer2/BtnDoor1");

        btn1.Pressed += () => Btn_Pressed(btn1);
        btn2.Pressed += () => Btn_Pressed(btn2);
        btn3.Pressed += () => Btn_Pressed(btn3);
        btn4.Pressed += () => Btn_Pressed(btn4);

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
        if (SelectedButton == null)
            return;

        Vector2I atlasCoords = (Vector2I)SelectedButton.GetMeta("Atlas");

        if (atlasCoords == new Vector2I(-1, -1))
        {
            DeleteBuildItem();
            return;
        }

        WorldMap map = WorldMain.Instance.Map;
        Vector2I mouseCoords = map.BuildingFloor.LocalToMap(WorldMain.Instance.Camera.GetGlobalMousePosition());

        //Tür
        if(SelectedButton == btn4)
        {
            if(map.BuildingWalls.GetCellAtlasCoords(mouseCoords) != new Vector2I(-1, -1) && map.BuildingWalls.GetCellSourceId(mouseCoords) == 0)
            {
                Vector2I atlasCell = map.BuildingWalls.GetCellAtlasCoords(mouseCoords);
                Vector2I cellTop = new Vector2I(1, 0);
                Vector2I cellBot = new Vector2I(1, 2);

                if (atlasCell == cellTop || atlasCell == cellBot)
                {
                    if(atlasCell == cellTop)
                        map.BuildingWalls.SetCell(mouseCoords, 1, new Vector2I(1, 0), 0);
                    else
                        map.BuildingWalls.SetCell(mouseCoords, 1, new Vector2I(0, 0), 0);
                }
            }
        }//Alle anderen
        else if (map.BuildingFloor.GetCellAtlasCoords(mouseCoords) == new Vector2I(-1, -1))
        {
            map.BuildingFloor.SetCell(mouseCoords, 1, atlasCoords, 0);

            BuildItem item = BuildItem.CreateBuildItem(SelectedButton, mouseCoords);
            item.Position = map.BuildingFloor.MapToLocal(mouseCoords);
            WorldMain.Instance.AddChild(item);
        }
    }

    public void DeleteBuildItem()
    {
        WorldMap map = WorldMain.Instance.Map;
        Vector2I mouseCoords = map.BuildingFloor.LocalToMap(WorldMain.Instance.Camera.GetGlobalMousePosition());

        if (map.BuildingFloor.GetCellAtlasCoords(mouseCoords) != new Vector2I(-1, -1) && map.BuildingFloor.GetCellSourceId(mouseCoords) == 1)
        {
            map.BuildingFloor.EraseCell(mouseCoords);
            map.BuildingWalls.EraseCell(mouseCoords);

            BuildItem item = null;
            foreach(Node node in GetTree().GetNodesInGroup("Working"))
            {
                if(node is BuildItem buildItem)
                {
                    if (buildItem.WorldCoords == mouseCoords)
                    {
                        item = buildItem;
                        break;
                    }
                }
            }

            if (item != null)
                item.QueueFree();
        }
    }
}
