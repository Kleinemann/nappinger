using Godot;
using System;


public partial class Weapon : Node2D
{
    [Export] public Texture2D WeaponSprite;
    Sprite2D Sprite;
    public CollisionShape2D CollisionShape;

    readonly Vector2 rightPos = new Vector2(17, -13);
    readonly Vector2 leftPos = new Vector2(-17, -10);
    readonly Vector2 upPos = new Vector2(0, -31);
    readonly Vector2 downPos = new Vector2(-2, 0);

    readonly float rightRot = -90;
    readonly float leftRot = 90;
    readonly float upRot = 180;
    readonly float downRot = 0;

    public override void _Ready()
    {
        Hide();
        Sprite = GetNode<Sprite2D>("Sprite2D");
        CollisionShape = GetNode<CollisionShape2D>("WeaponHitBox/CollisionShape2D");
        Area2D area = GetNode<Area2D>("WeaponHitBox");
        area.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        GD.Print("Hit: " + body.Name);
        if(body.IsInGroup("Breakable"))
        {
            BreakableObject obj = body as BreakableObject;
            obj.Healt -= 2;
        }        
    }

    public override void _Process(double delta)
    {
        WeaponPosition();
    }

    public void WeaponPosition()
    {
        string dir = (GetParent<Player>()).Direction;
        switch (dir)
        {
            case "r":
                Position = rightPos;
                RotationDegrees = rightRot;
                Sprite.FlipH = false;
                break;
            case "l":
                Position = leftPos;
                RotationDegrees = leftRot;
                Sprite.FlipH = true;
                break;
            case "u":
                Position = upPos;
                RotationDegrees = upRot;
                Sprite.FlipH = false;
                break;
            case "d":
                Position = downPos;
                RotationDegrees = downRot;
                Sprite.FlipH = true;
                break;
        }
    }
}