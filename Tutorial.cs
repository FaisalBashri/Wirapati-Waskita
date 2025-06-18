using Godot;
using System;

public partial class Tutorial : Node3D
{
	public override void _Ready()
	{
	}
	private void OnSelesaiPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
}
