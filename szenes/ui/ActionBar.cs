using Godot;
using Godot.Collections;

public partial class ActionBar : Control
{
    public ActionBarItems ItemResource;
    public Array<InventarSlot> Slots = new Array<InventarSlot>();

    public override void _Ready()
    {
        //Items
        ItemResource = GD.Load<ActionBarItems>("res://resources/ui/ActionBarItems.tres");

        //Slots
        GridContainer gc = GetNode<GridContainer>("NinePatchRect/GridContainer");
        Array<Node> nodes = new Array<Node>();
        foreach(Node s in gc.GetChildren())
            Slots.Add((InventarSlot)s);

        InitSlots();
    }

    public void InitSlots()
    {
        foreach(int i in GD.Range(Mathf.Min(ItemResource.Items.Count, Slots.Count)))
        {
            Slots[i].Update(ItemResource.Items[i], 0);
        }
    }

    public void AddItem(int id , int value = 1)
    {
        int index = -1;
        for(int i =0; i< Slots.Count; i++)
        {
            InventarSlot s = Slots[i];
            if(s.Item == null || s.Item.ID == id)
            {
                index = i;
                break;
            }
        }

        if (index > -1)
        {
            ItemBase ib = ActionBarItems.ItemList[id];
            ItemResource.Items[index] = ib;
            Slots[index].Update(ib, value);
        }
        else
            GD.Print("KEIN PLATZ IM INVENTAR");
    }
}
