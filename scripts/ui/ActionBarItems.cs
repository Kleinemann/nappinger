using Godot;
using Godot.Collections;


public partial class ActionBarItems : Resource
{
    [Export] public Array<ItemBase> Items { get; set; }
}