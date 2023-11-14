using System;
using Godot;

public partial class WaitingLabel : Label
{
	private Timer _dotsTimer;
	private Timer _startGameTimer;
	private Main.State _mainState = Main.State.IDLE;
	private int _currentDotsIndex = 0;
	private string _currentMsg;

	[Signal]
	public delegate void StartGameTimerTimeoutEventHandler();

	public override void _Ready()
	{
		_dotsTimer = GetNode<Timer>("%DotsTimer");
		_dotsTimer.Timeout += OnDotsTimerTimeout;
		_startGameTimer = GetNode<Timer>("%StartGameTimer");
		_startGameTimer.Timeout += OnStartGameTimerTimeout;
		
		SetPhysicsProcess(false);
	}

    public override void _PhysicsProcess(double delta)
    {
		switch (_mainState)
		{
			case Main.State.STARTING_GAME:
				Text = _currentMsg + Mathf.CeilToInt(_startGameTimer.TimeLeft);
				break;

			default:
			break;
		}
    }

    private void UpdateWaitingMessage(Main.State state)
	{
		_mainState = state;
		_currentDotsIndex = 0;
		_dotsTimer.Stop();
		_startGameTimer.Stop();

		switch (state)
		{
			case Main.State.WAITING_PLAYER:
				_currentMsg = "Waiting for a player";
				_dotsTimer.Start();
				break;
			case Main.State.SEARCHING_GAME:
				_currentMsg = "Searching a game";
				_dotsTimer.Start();
				break;
			case Main.State.STARTING_GAME:
				SetPhysicsProcess(true);
				_currentMsg ="Game starts in ";
				_startGameTimer.Start();
				break;
			default:
				break;
		}

		Text = _currentMsg;
		CustomMinimumSize = Size;
	}

	public void StartWaiting(Main.State state)
	{
		UpdateWaitingMessage(state);
		Visible = true;
	}
	public void StoptWaiting()
	{
		_dotsTimer.Stop();
		_startGameTimer.Stop();
		
		Visible = false;
		_currentDotsIndex = 0;
	}


	private void OnDotsTimerTimeout()
	{
		Text = _currentMsg + new string('.', Mathf.PosMod(_currentDotsIndex,4));
		_currentDotsIndex++;
	}
	private void OnStartGameTimerTimeout()
	{
		EmitSignal(SignalName.StartGameTimerTimeout);
		SetPhysicsProcess(false);
	}
}
