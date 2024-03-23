using Godot;
using System;

public partial class Package : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Position -= new Vector2(0.4f, 0) * 100 * (float)delta;
	}
}
