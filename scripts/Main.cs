using System;
using Godot;

public partial class Main : Node
{
	[Export]
	private PackedScene _gamePackedScene;
	public enum State
	{
		IDLE, PLAYING, WAITING_PLAYER, SEARCHING_GAME, STARTING_GAME
	}

	private State _state = State.IDLE;
	private MainMenuUI _mainMenuUI;
	private MultiPlayerManager _multiPlayerManager;

	public override void _Ready()
	{
		_mainMenuUI = GetNode<MainMenuUI>("%MainMenuUI");
		_multiPlayerManager = GetNode<MultiPlayerManager>("%MultiPlayerManager");

		_multiPlayerManager.MinNumPeersConnected += OnMinNumPeersConnected;

		_mainMenuUI.HostButtonDown += OnMainMenuHostButtonDown;
		_mainMenuUI.JoinButtonDown += OnMainMenuJoinButtonDown;
		_mainMenuUI.StopButtonDown += OnMainMenuStopButtonDown;
		_mainMenuUI.StartGameTimerTimeout += OnMainMenuStartGameTimerTimeout;
	}

	private void OnMainMenuHostButtonDown(string peerName, string gameName)
	{
		_multiPlayerManager.HostGame(peerName, gameName);
	}
	private void OnMainMenuJoinButtonDown(string peerName, string gameName)
	{
		_multiPlayerManager.JoinGame(peerName, gameName);
	}
	private void OnMinNumPeersConnected()
	{
		Rpc(MethodName.StartWaiting, Enum.GetName(State.STARTING_GAME));
	}
	private void OnMainMenuStopButtonDown()
	{
		GD.Print("Stop");
	}
	private void OnMainMenuStartGameTimerTimeout()
	{
		if (!Multiplayer.IsServer())
			return;

		foreach(PlayerRes playerRes in GlobalGameManager.PlayersResList)
			GD.Print(playerRes.Name + "is playing");
		
		Rpc(MethodName.StartGame);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartGame()
	{	
		_mainMenuUI.Hide();

		Game gameScene = _gamePackedScene.Instantiate<Game>();
		GetNode<CanvasLayer>("MainMenuLayer").AddChild(gameScene);
		// GetTree().ChangeSceneToPacked(_gamePackedScene);
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=true, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	private void StartWaiting(string stateStr)
	{
		State state = (State)Enum.Parse(typeof(State), stateStr);
		_state = state;
		_mainMenuUI.StartWaiting(state);
	}
}
