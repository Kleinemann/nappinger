using Godot;
using System;

public partial class Npc : RigidBody3D
{
	Label3D label;

	double last = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label3D>("Label3D");
	}

	void ShowText(string text)
	{
		label.Text = text;
		label.Visible = true;
	}

    private void HideLabel()
    {
        label.Text = string.Empty;
        label.Visible = false;
		last = -1;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(GD.RandRange(0, 100) == 1)
		{
			string[] t = new string[] { "Hallo", "Hilfe", "Test", "Hunger", "Hund", "Bye" };
			string text = t[GD.RandRange(0, t.Length - 1)];

            ShowText(text);
			last = 1;
		}

		if(last > -1)
		{
			last -= delta;

            if (last <= 0)
				HideLabel();
		}
	}
}
