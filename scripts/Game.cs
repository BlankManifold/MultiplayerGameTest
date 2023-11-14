using Godot;

public partial class Game : Node2D
{
	[Export]
	private PackedScene _playerPackedScene;

	public override void _Ready()
	{
		if (!Multiplayer.IsServer())
			return;

		foreach (PlayerRes playerRes in GlobalGameManager.PlayersResList)
		{
			Rpc(MethodName.SpawnPlayer, playerRes.Id, playerRes.Name);
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	public void SpawnPlayer(int id, string name)
	{
		Player player = _playerPackedScene.Instantiate<Player>();
		player.Init(id,name);

		GetNode<Node2D>("Players").AddChild(player, true);
	}
}
