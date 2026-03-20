using Godot;
using System;

public partial class ItemBase : Sprite2D
{
    public int ID { get; set; }
    public string Namen { get; set; }
    public int Value { get; set; } = 1;
}
