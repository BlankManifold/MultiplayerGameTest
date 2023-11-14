using Godot;

public partial class MainMenuUI : Control
{
	
	private WaitingLabel _waitingLabel;
	private CreateGamePopup _createGamePopup;
	private Button _stopButton;


	[Signal]
	public delegate void JoinButtonDownEventHandler(string peerName, string gameName);
	[Signal]
	public delegate void HostButtonDownEventHandler(string peerName, string gameName);
	[Signal]
	public delegate void StopButtonDownEventHandler();
	[Signal]
	public delegate void StartGameTimerTimeoutEventHandler();

	public override void _Ready()
	{
		_waitingLabel = GetNode<WaitingLabel>("%WaitingLabel");
		_stopButton = GetNode<Button>("%StopButton");
		_createGamePopup = GetNode<CreateGamePopup>("%CreateGamePopup");

		_createGamePopup.CreateGameButtonDown += OnCreateGamePopupButtonDown;
		_createGamePopup.BackButtonDown += OnBackButtonDown;
		_waitingLabel.StartGameTimerTimeout += () => EmitSignal(SignalName.StartGameTimerTimeout);
	}

	public void StartWaiting(Main.State state)
	{
		_waitingLabel.StartWaiting(state);
	}

	public void _on_join_game_button_button_down()
	{
		EmitSignal(SignalName.JoinButtonDown, "joinedPeerName", "joinedGameName");
		_stopButton.Disabled = false;
		// _waitingLabel.StartWaiting(Main.State.STARTING_GAME);
	}
	public void _on_host_game_button_button_down()
	{
		_createGamePopup.Popup();
	}
	public void _on_stop_button_button_down()
	{
		EmitSignal(SignalName.StopButtonDown);
		_stopButton.Disabled = true;
		_waitingLabel.StoptWaiting();
	}

	public void OnCreateGamePopupButtonDown(string peerName, string gameName)
	{
		_createGamePopup.Hide();
		_stopButton.Disabled = false;

		_waitingLabel.StartWaiting(Main.State.WAITING_PLAYER);
		EmitSignal(SignalName.HostButtonDown, peerName, gameName);
	}
	public void OnBackButtonDown()
	{
		_createGamePopup.Hide();
		
	}
}



