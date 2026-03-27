using Godot;
using System;

public partial class Player : CharacterBody2D
{
    int Speed = 250;

    public override void _Process(double delta)
    {
        PlayerMovement();
    }

    public void PlayerMovement()
    {
        Velocity = Input.GetVector("left", "rigth", "up", "down");
        Velocity = Velocity * Speed;

        MoveAndSlide();
    }
}
