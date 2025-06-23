using Godot;
using System;

public partial class TPlayer : CharacterBody3D
{
	public const float Speed = 20.0f;
	public const float JumpVelocity = 1f;

	[Export]
	Camera3D Camera;
	[Export]
	Boolean CameraIsTop;

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		velocity.Y = JumpVelocity * -Speed/5;

		// Handle Jump.
		if (Input.IsActionPressed("ui_accept"))
		{
			velocity.Y = JumpVelocity * Speed;
		}
		else
		{
            velocity.Y += Mathf.MoveToward(Velocity.Y, 0, Speed);
        }
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

		Velocity = velocity;
		MoveAndSlide();

        if (Input.IsActionPressed("ui_v"))
            CameraIsTop = !CameraIsTop;

        if (Camera != null)
		{
			if (CameraIsTop)
			{
				Vector3 cPos = new Vector3(Position.X, 50, Position.Z);
                Camera.Position = cPos;
				Camera.Rotation = new Vector3(Mathf.DegToRad(-90), 0, 0);
            }
			else
			{
                Vector3 cPos = new Vector3(Position.X, Position.Y + 5, Position.Z + 3);
                Camera.Rotation = new Vector3(Mathf.DegToRad(-25), 0, 0);
                Camera.Position = cPos;
            }

        }
	}
}
