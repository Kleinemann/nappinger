using Godot;
using System;

[GlobalClass]
public partial class ObjectBaseResource : Resource
{
    [Export] public int ID { get; set; }
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Texture { get; set; }

}
