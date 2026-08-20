using Godot;
using System;
using static WorldMap;

public partial class PlantMenu : Panel
{
    Button btn1;
    Button btn2;
    Button btn3;
    Button btn4;
    public static PlantMenu Instance;
    public static Button SelectedButton;

    public override void _Ready()
    {
        btn1 = GetNode<Button>("HFlowContainer/BtnWater");
        btn2 = GetNode<Button>("HFlowContainer/BtnSand");
        btn3 = GetNode<Button>("HFlowContainer/BtnGras");
        btn4 = GetNode<Button>("HFlowContainer/BtnDirt");

        btn1.Pressed += () => Btn_Pressed(btn1);
        btn2.Pressed += () => Btn_Pressed(btn2);
        btn3.Pressed += () => Btn_Pressed(btn3);
        btn4.Pressed += () => Btn_Pressed(btn4);

        SelectedButton = btn4;

        Hidden += HidePlantMenu;
    }

    private void Btn_Pressed(Button button)
    {
        SelectedButton = button;
    }

    public void ShowPlantMenu()
    {
        Instance = this;
        Show();
    }

    public void HidePlantMenu()
    {
        Instance = null;
        Hide();
    }

    public void CreatePlantItem()
    {
        Vector2I atlasCoords = (Vector2I)SelectedButton.GetMeta("Atlas");

        WorldMap map = WorldMain.Instance.Map;
        Vector2I mouseCoords = map.WorldLayer.LocalToMap(WorldMain.Instance.Camera.GetGlobalMousePosition());

        map.WorldLayer.SetCell(mouseCoords, 0, atlasCoords);

        //TODO: Ränder korigieren !

        Chunk chunk = map.GetChunk(mouseCoords);

        foreach (Vector2I ce in chunk.GetNeigbours(mouseCoords))
        {
            chunk.RefreshOffset(ce);
        }

    }
}
