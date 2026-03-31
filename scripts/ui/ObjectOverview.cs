using Godot;
using System;

public partial class ObjectOverview : Control
{

    // Items
    VBoxContainer ObjectBox;
    Label ObjectLabel;
    Label ObjectValue;
    TextureRect ObjectPicture;
    Label ObjectCoord;

    public override void _Ready()
    {
        //Items
        ObjectBox = GetNode<VBoxContainer>("ObjectBox");
        ObjectLabel = GetNode<Label>("ObjectBox/HBoxContainer2/ObjectName");
        ObjectValue = GetNode<Label>("ObjectBox/HBoxContainer2/ObjectValue");
        ObjectPicture = GetNode<TextureRect>("ObjectBox/HBoxContainer/PanelContainer/ObjectPicture");
        ObjectCoord = GetNode<Label>("ObjectBox/HBoxContainer/ObjectCoord");
        Visible = false;
    }

    public void Update()
    {
        //GameObject go = WorldMain.Instance.Map.Marker.CurrentObject;
        //if (go != null)
        //{
        //    Visible = true;
        //    ObjectLabel.Text = go.ObjectName;
        //    //ObjectValue.Text = go.Value.ToString();
        //    ObjectPicture.Texture = go.Texture;
        //    ObjectCoord.Text = $" ( {go.Position.X} | {go.Position.Y} )";
        //}
        //else
        //{
        //    Visible = false;
        //}
    }
}
