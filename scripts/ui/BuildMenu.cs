using Godot;

public partial class BuildMenu : Panel
{
    Button btn1;
    Button btn2;

    public static Button SelectedItem;

    public override void _Ready()
    {
        btn1 = GetNode<Button>("HFlowContainer/BtnFloor1");
        btn2 = GetNode<Button>("HFlowContainer/BtnFloor2");

        btn1.Pressed += () => Btn_Pressed(btn1);
        btn2.Pressed += () => Btn_Pressed(btn1);

        Hidden += BuildMenu_Hidden;
    }

    private void BuildMenu_Hidden()
    {
        SelectedItem = null;
    }

    private void Btn_Pressed(Button button)
    {
        SelectedItem = button;
    }

    public void ShowBuildMenu()
    {
        if (btn1.ButtonPressed)
            SelectedItem = btn1;
        else
            SelectedItem = btn2;
        Show();
    }

    public void HideBuildMenu()
    {
        SelectedItem = null;
        Hide();
    }
}
