using Godot;
using System;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
	}

	private void OnTutorialPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Tutorial.tscn");
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/LevelSelection.tscn");
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
