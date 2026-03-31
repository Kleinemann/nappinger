using Godot;
using System;

public partial class Player : CharacterBody2D
{
    AnimatedSprite2D Animator;
    AnimatedSprite2D AnimatorShadow;
    int Speed = 150;
    string direction = "d";
    public static Player SelectetPlayer;

    public override void _Process(double delta)
    {
        Animator = GetNode<AnimatedSprite2D>("Sprite2D");
        AnimatorShadow = GetNode<AnimatedSprite2D>("Sprite2DShadow");
        PlayerMovement();
        PlayerAnimation();
    }

    public void PlayerMovement()
    {
        if (SelectetPlayer != this)
            return;

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

        string animationName;

        if(Velocity == Vector2.Zero)
            animationName = "idle_" + direction;
        else
            animationName= "walk_" + direction;

        Animator.Play(animationName);
        AnimatorShadow.Play(animationName);
    }
}
