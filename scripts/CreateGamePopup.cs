using Godot;

public partial class CreateGamePopup : PopupPanel
{
	private LineEdit _gameNameLineEdit;
	private LineEdit _peerNameLineEdit;

	[Signal]
	public delegate void CreateGameButtonDownEventHandler(string peerName, string gameName);
	[Signal]
	public delegate void BackButtonDownEventHandler();

    public override void _Ready()
    {
		_gameNameLineEdit = GetNode<LineEdit>("%GameNameLineEdit");
		_peerNameLineEdit = GetNode<LineEdit>("%PeerNameLineEdit");
    }

	private void _on_create_game_button_button_down()
	{
		EmitSignal(SignalName.CreateGameButtonDown, _peerNameLineEdit.Text, _gameNameLineEdit.Text);
	}
	private void _on_back_button_button_down()
	{
		EmitSignal(SignalName.BackButtonDown);
	}
	private void _on_close_requested()
	{
		EmitSignal(SignalName.BackButtonDown);
	}
}
