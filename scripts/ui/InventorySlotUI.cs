using Godot;

public partial class InventorySlotUI : Panel
{
    Sprite2D Icon;
    Label Amount;

    public override void _Ready()
    {
        Icon = GetNode<Sprite2D>("CenterContainer/Panel/Icon");
        Amount = GetNode<Label>("CenterContainer/Panel/Label");
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if(slot == null || slot.Item == null)
        {
            Icon.Texture = null;
            Icon.Hide();
            Amount.Text = "";
            Amount.Hide();
        }
        else
        {
            Icon.Show();
            Icon.Texture = slot.Item.Icon;

            if(slot.Amount > 1)
            {
                Amount.Show();
                Amount.Text = slot.Amount.ToString();
            }
            else
            {
                Amount.Hide();
                Amount.Text = "";
            }
        }
    }
}
