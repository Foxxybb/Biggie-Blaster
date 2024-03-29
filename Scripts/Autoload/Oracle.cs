using Godot;
using System;

public partial class Oracle : Node
{
	public static Oracle Instance;

	public Node CurrentScene { get; set; }
	string nextScenePath; // used to store path to delay scene change until transition animation finishes

	AnimationPlayer transition;
	MyCamera cam;

	public bool myDebug = true;

	public double displayTime;
	public double PBTime = 60;
	public bool displayTimerOn;
	public bool playerDead;

	int endTick;
	bool gameIsOver;
	public bool playerHasControl = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Engine.MaxFps = 60;
		GD.Seed(12345);

		Events.Instance.GameSceneReady += () => OnNewScene();
		Events.Instance.FinalKill += () => TriggerEndSequence();

		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (displayTimerOn){
			displayTime -= delta;
		}

		if (Input.IsActionJustPressed("reset_action") && playerHasControl)
		{
			//ResetScene();
			// if current scene is not gameplay scene, change scene to Tutorial
			// else, change scene to main game scene
			var curScene = GetNode<GameScene>("/root/Scene");

			if (curScene.isGameplayScene){
				ChangeScene("res://Scenes/testScene.tscn");
			} else {
				ChangeScene("res://Scenes/training.tscn");
			};

			if (curScene.soundOnExit != null){
				SoundManager.Instance.StopMusic();
				SoundManager.Instance.PlaySound(curScene.soundOnExit);
			}
		}

		if (Input.IsActionJustPressed("test_action"))
		{
			// if (DisplayServer.WindowGetMode() == (DisplayServer.WindowMode.ExclusiveFullscreen)){
			// 	DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			// } else {
			// 	DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			// }
		}

		if (gameIsOver){
			endTick += 1;

			switch (endTick){
				case 179:
					// show victory text
					GetNode<AnimatedSprite2D>("/root/Scene/Victory").Play("victory");
					break;
				case 180:
					SoundManager.Instance.PlaySound(SoundManager.Instance.voice_victory);
					// remove top barrier
					GetNode<StaticBody2D>("/root/Scene/GroundT").QueueFree();
					break;
				case 240:
					if (!GetNode<Player>("/root/Scene/Player").jumpMode){
						GetNode<Player>("/root/Scene/Player").ModeChange();
					} 
					break;
				case 300:
					//rocket player off?
					GetNode<Player>("/root/Scene/Player").autoFireOn = true;
					break;
				case 1000:
					// end game
					ChangeScene("res://Scenes/about.tscn");
					break;
				case 1060:
					endTick = 0;
					gameIsOver = false;
					playerHasControl = true;
					SoundManager.Instance.musicPlayer.VolumeDb = -8;
					SoundManager.Instance.PlayMusic(SoundManager.Instance.introMusic);
					break;
				default:
					break;
			}
		}

	}

	void TriggerEndSequence(){
		gameIsOver = true;
		playerHasControl = false;
		GetNode<Player>("/root/Scene/Player").ResetCD();
	}

	void OnNewScene(){
		cam = CurrentScene.GetNode<MyCamera>("MyCamera");

		transition = GetNode<AnimationPlayer>("/root/Scene/MyCamera/Transition/TransitionAnimator");
		transition.AnimationFinished += _on_transition_end;

		displayTimerOn = true;
		displayTime = 60;
		playerDead = false;
	}

	public void ResetScene(){
		GD.Print("Reset Scene");
		// play transition out
		transition.Play("transition_out");
		
		// reset scene
		nextScenePath = GetTree().CurrentScene.SceneFilePath;
	}

	public void ChangeScene(string path){
		GD.Print("Change Scene");
		// play transition out
		transition.Play("transition_out");

		nextScenePath = path;
	}

	// Scene change script
	public void GoToScene(string path)
	{
		// call is deferred to prevent crashing when changing scenes
		CallDeferred(nameof(DeferredGotoScene), path);

		// play transition_in animation
		transition.Play("transition_in");
	}

	public void DeferredGotoScene(string path)
	{
		// It is now safe to remove the current scene
		CurrentScene.Free();

		// Load a new scene.
		var nextScene = (PackedScene)GD.Load(path);

		// Instance the new scene.
		CurrentScene = nextScene.Instantiate();

		// Add it to the active scene, as child of root.
		GetTree().Root.AddChild(CurrentScene);

		// Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
		GetTree().CurrentScene = CurrentScene;
	}

	void _on_transition_end(StringName anim_name)
	{
		// if transitioning OUT, trigger scene change
		if (anim_name == "transition_out"){
			GoToScene(nextScenePath);
		}
	}
}
