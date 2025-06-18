using Godot;
using System;

public partial class WinBoard : Control
{
	private void OnSelesaiPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
}
