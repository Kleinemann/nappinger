using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;

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
            Slots[i].Update(null);
        }
    }

    public int AddItem(int id , int count = 1)
    {
        ItemBase ib = ItemBase.GetItem(id);
        ib.Count = count;

        return AddItem(ib);
    }

    public int AddItem(ItemBase ib)
    {
        int iEmpty = -1;
        int iItem = -1;

        for(int i = 0; i < ItemResource.Items.Count; i++)
        {
            ItemBase tmpItem = ItemResource.Items[i];

            if (tmpItem == null && iEmpty < 0)
            {
                iEmpty = i;
            }

            if (tmpItem != null
                && tmpItem.ID == ib.ID
                && tmpItem.Count < ItemBase.MaxCount)
            {
                iItem = i;
            }
        }

        if(iItem >=0)
        {
            ItemBase item = ItemResource.Items[iItem];
            var ret = item.Add(ib.Count);
            Slots[iItem].Update(item);

            //Overflow to next Slot
            if (ret > 0)
            {
                ib.Count = ret;
                return AddItem(ib);
            }

            return ret;
        }


        if(iEmpty >= 0)
        {
            {
                ItemResource.Items[iEmpty] = ib;

                if(ItemResource.Items[iEmpty].Count > ItemBase.MaxCount)
                {
                    ItemBase ibNew = ib.Duplicate() as ItemBase;
                    ibNew.Count = ib.Count - ItemBase.MaxCount;
                    ib.Count = ItemBase.MaxCount;
                    Slots[iEmpty].Update(ib);
                    return AddItem(ibNew);
                }

                Slots[iEmpty].Update(ib);
                return 0;
            }
        }

        return ib.Count;
    }
}
