using Godot;
using nappinger.scripts;
using System;
using System.Linq;
using static WorldMain;

public partial class Ui : Control
{
    public static Ui Instance;

    // Items
    VBoxContainer ObjectBox;
    Label ObjectLabel;
    Label ObjectValue;
    TextureRect ObjectPicture;
    TextureRect ObjectIsHuman;
    Label ObjectCoord;

    public override void _Ready()
    {
        Instance = this;

        //Items
        ObjectBox = GetNode<VBoxContainer>("ObjectBox");
        ObjectLabel = GetNode<Label>("ObjectBox/HBoxContainer2/ObjectName");
        ObjectValue = GetNode<Label>("ObjectBox/HBoxContainer2/ObjectValue");
        ObjectPicture = GetNode<TextureRect>("ObjectBox/HBoxContainer/PanelContainer/ObjectPicture");
        ObjectIsHuman = GetNode<TextureRect>("ObjectBox/HBoxContainer/IsHuman");
        ObjectCoord = GetNode<Label>("ObjectBox/HBoxContainer/ObjectCoord");
        Visible = false;
    }

    public void Update()
    {
        GameObject go = WorldMain.Instance.Map.Marker.CurrentObject;
        if(go != null)
        {
            Visible = true;
            ObjectLabel.Text = go.ObjectName;
            ObjectValue.Text = go.Value.ToString();
            ObjectPicture.Texture = go.Icon;
            ObjectIsHuman.Visible = go.ObjectType == ObjectTypeEnum.PLAYER;
            ObjectCoord.Text = $" ( {go.Position.X} | {go.Position.Y} )";
        }
        else
        {
            Visible = false;
        }
    }
}