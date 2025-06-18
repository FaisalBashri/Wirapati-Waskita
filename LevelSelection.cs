using Godot;
using System;

public partial class LevelSelection : Control
{
	public override void _Ready()
	{
	}
	
	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
}
