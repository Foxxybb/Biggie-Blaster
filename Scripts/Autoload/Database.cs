using Godot;
using System;

public partial class Database : Node
{
	public static Database Instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		
	}

}
