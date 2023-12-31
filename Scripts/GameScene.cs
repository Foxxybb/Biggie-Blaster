using Godot;
using System;

public partial class GameScene : Node2D
{
	[Export] public AudioStreamWav musicForScene;
	[Export] public AudioStreamWav soundForScene;
	[Export] public AudioStreamWav soundOnExit;

	[Export] public bool isGameplayScene;
	[Export] public bool isDemoScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Events.Instance.EmitSignal("GameSceneReady");

		if (musicForScene != null){
			SoundManager.Instance.PlayMusic(musicForScene);
		}

		if (soundForScene != null){
			SoundManager.Instance.PlaySound(soundForScene);
		}

		if (isDemoScene){
			Oracle.Instance.playerHasControl = false;
		} else {
			Oracle.Instance.playerHasControl = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
