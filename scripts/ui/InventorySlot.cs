using Godot;

public partial class InventorySlot : Panel
{
    Sprite2D Icon;

    public override void _Ready()
    {
        Icon = GetNode<Sprite2D>("CenterContainer/Panel/Icon");
    }

    public void UpdateSlot(InventoryItem item)
    {
        if(item == null)
        {
            Icon.Texture = null;
            Icon.Hide();
        }
        else
        {
            Icon.Show();
            Icon.Texture = item.Icon;
        }
    }
}
