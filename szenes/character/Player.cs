using Godot;
using System;

public partial class Player : CharacterBody2D
{
    AnimatedSprite2D Animator;
    int Speed = 150;
    string direction = "d";


    public override void _Process(double delta)
    {
        Animator = GetNode<AnimatedSprite2D>("Sprite2D");
        PlayerMovement();
        PlayerAnimation();
    }

    public void PlayerMovement()
    {
        Velocity = Input.GetVector("left", "rigth", "up", "down");
        Velocity = Velocity * Speed;

        MoveAndSlide();
    }

    public void PlayerAnimation()
    {
        if (Velocity.X > 0) direction = "r";
        else if (Velocity.X < 0) direction = "l";
        else if (Velocity.Y > 0) direction = "d";
        else if (Velocity.Y < 0) direction = "u";

        if(Velocity == Vector2.Zero)
            Animator.Play("idle_" + direction);
        else
            Animator.Play("walk_" + direction);
    }
}
