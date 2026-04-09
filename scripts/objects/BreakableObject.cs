using Godot;
using System;

[GlobalClass]
public partial class BreakableObject : StaticBody2D
{
    [Export] public AnimatedSprite2D Sprite;
    [Export] public AnimatedSprite2D SpriteShadow;
    [Export] public CollisionShape2D CollisionShape;

    GameObjectDataBase _data = new GameObjectDataBase();

    #region GameObjectData
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

    public override void _Input(InputEvent @event)
    {
        ////Focus / defocus Player
        //if (@event.IsActionPressed("remove_value"))
        //{
        //    Healt--;
        //    if(Healt < 0)
        //        Healt = 0;
        //    UpdateAnimation();
        //}
    }
    #endregion

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
