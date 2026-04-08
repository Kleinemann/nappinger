using Godot;
using System;

[GlobalClass]
public partial class BreakableObject : StaticBody2D
{
    [Export] public AnimatedSprite2D Sprite;
    [Export] public AnimatedSprite2D SpriteShadow;
    [Export] public CollisionShape2D CollisionShape;

    GameObjectData Data = new GameObjectData();

    public override void _Input(InputEvent @event)
    {
        //Focus / defocus Player
        if (@event.IsActionPressed("kill"))
        {
            Data.Healt--;
            if(Data.Healt < 0)
                Data.Healt = 0;
            UpdateAnimation();
        }
    }

    public void UpdateAnimation()
    {
          float healt = (Data.Healt * 100 / Data.MaxHealt);

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
