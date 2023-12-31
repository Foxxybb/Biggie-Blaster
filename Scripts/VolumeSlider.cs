using Godot;
using System;

public partial class VolumeSlider : HSlider
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Value = AudioServer.GetBusVolumeDb(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// signal method
	private void _on_value_changed(double value)
	{
		AudioServer.SetBusVolumeDb(0, (float)value);
	}

}


