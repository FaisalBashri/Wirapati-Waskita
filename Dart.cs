using Godot;
using System;

public partial class Dart : RigidBody3D
{
	[Export] public int Damage = 10;
	
	public override void _Ready()
	{
		this.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is ProtoController pc)
		{
			pc.TakeDamage(Damage);
		}
		QueueFree();
	}
}
