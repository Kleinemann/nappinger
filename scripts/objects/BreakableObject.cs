using Godot;
using System;

[GlobalClass]
public partial class BreakableObject : StaticBody2D
{
    [Export] public AnimatedSprite2D Sprite;
    [Export] public AnimatedSprite2D SpriteShadow;
    [Export] public CollisionShape2D CollisionShape;

    public static BreakableObject SelectedObject;

    GameObjectDataBase _data = new GameObjectDataBase();

    #region GameObjectData
    [Export]
    public string ObjectName
    {
        get => _data.Name;
        set => _data.Name = value;
    }

    public int Healt
    {
        get => _data.Healt;
        set
        {
            _data.Healt = value;
            if (_data.Healt < 0)
                _data.Healt = 0;
        }
    }

    public int MaxHealt
    {
        get => _data.MaxHealt;
        set
        {
            _data.MaxHealt = value;
            if (_data.MaxHealt < 1)
                _data.MaxHealt = 1;
        }
    }

    public override void _Ready()
    {
        Area2D area = GetNode<Area2D>("Area2D");
        area.InputEvent += OnInputEvent;
    }

    #endregion


    public void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Object CLICK");
            Player.SelectetPlayer = null;
            SelectedObject = this;
        }
    }


    public void UpdateAnimation()
    {
          float healt = (Healt * 100 / MaxHealt);

        if (healt == 0) Sprite.Play("3");
        else if (healt < 40) Sprite.Play("2");
        else if (healt < 70) Sprite.Play("1");
        else Sprite.Play("0");

        if (healt == 0) SpriteShadow.Play("3");
        else if (healt < 40) SpriteShadow.Play("2");
        else if (healt < 70) SpriteShadow.Play("1");
        else SpriteShadow.Play("0");

    }
}
