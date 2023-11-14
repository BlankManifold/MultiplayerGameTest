using Godot;

public partial class Player : StaticBody2D
{
	[Export]
	private float _rotationStep = Mathf.Pi / 4;
	[Export]
	private Godot.Collections.Dictionary<string, float> _lerpWeightDict = new Godot.Collections.Dictionary<string, float>()
	{
		{"pos",0.5f},
		{"rot",0.5f}
	};
	private string _name;
	private int _id;
	private int _idAuthority;
	private MultiplayerSynchronizer _multiplayerSynchronizer;

	[Export]
	private Godot.Collections.Dictionary<string, Variant> _syncInfo = new Godot.Collections.Dictionary<string, Variant>()
	{
		{"pos",new Vector2(0,0)},
		{"rot",0}
	};

	public void Init(int id, string name, float rotationStep = Mathf.Pi / 4, float rotation = 0)
	{
		_id = id;
		_name = name;
		_rotationStep = rotationStep;

		Name = id.ToString();
		GlobalRotation = rotation;

	}
	public override void _EnterTree()
	{
		_multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		_multiplayerSynchronizer.SetMultiplayerAuthority(_id);
	}
	public override void _Ready()
	{
		// _multiplayerSynchronizer.SetMultiplayerAuthority(_id);
		GetNode<Label>("Label").Text = _name;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_multiplayerSynchronizer.IsMultiplayerAuthority())
		{
			GlobalPosition = GetGlobalMousePosition();

			if (Input.IsActionJustPressed("rotate"))
			{
				Rotate(_rotationStep);
			}
			UpdateSyncInfo();
		}
		else
		{
			UpdateInfoFromSyncInfo();
		}
	}

	private void UpdateSyncInfo()
	{
		_syncInfo["pos"] = GlobalPosition;
		_syncInfo["rot"] = GlobalRotation;
	}
	private void UpdateInfoFromSyncInfo()
	{
		GlobalPosition = GlobalPosition.Lerp((Vector2)_syncInfo["pos"] , _lerpWeightDict["pos"]);
		GlobalRotation = (float)_syncInfo["rot"];
	}
}
