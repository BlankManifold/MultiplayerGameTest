using Godot;

public partial class Ball: RigidBody2D
{
	[Export]
	private float _speed = 50f;
	[Export]
	private Godot.Collections.Dictionary<string, float> _lerpWeightDict = new Godot.Collections.Dictionary<string, float>()
	{
		{"pos",0.5f},
	};
	[Export]
	private Godot.Collections.Dictionary<string, Variant> _syncInfo = new Godot.Collections.Dictionary<string, Variant>()
	{
		{"pos",new Vector2(0,0)}
	};

	public override void _Ready()
	{
		if (!Multiplayer.IsServer())
		{
			Freeze = true;
			return;
		}

		GD.Randomize();
		LinearVelocity = new Vector2(GD.Randf(),GD.Randf()).Normalized()*_speed;
	}
    public override void _PhysicsProcess(double delta)
    {
		if (Multiplayer.IsServer())
			UpdateSyncInfo();
		else
		{
			UpdateInfoFromSyncInfo();
		}
    }

	private void UpdateSyncInfo()
	{
		_syncInfo["pos"] = GlobalPosition;
	}
	private void UpdateInfoFromSyncInfo()
	{
		GlobalPosition = GlobalPosition.Lerp((Vector2)_syncInfo["pos"] , _lerpWeightDict["pos"]);
	}
}
