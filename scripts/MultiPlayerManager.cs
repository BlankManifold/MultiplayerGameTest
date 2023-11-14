using Godot;

public partial class MultiPlayerManager : Node
{
	private string _ipAddress = "127.0.0.1";
	private int _port = 8901;
	private ENetConnection.CompressionMode _compressionMode = ENetConnection.CompressionMode.RangeCoder;
	private ENetMultiplayerPeer _peer;
	private PlayerRes _playerRes;
	private int _minNumPeers = 2;

	public override void  _Ready()
	{
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		Multiplayer.ConnectedToServer += OnConnectedToServer;
		Multiplayer.ServerDisconnected += OnDisconnectedFromServer;
		Multiplayer.ConnectionFailed += OnConnectionFailed;
	}

	[Signal]
	public delegate void MinNumPeersConnectedEventHandler();

    public void HostGame(string peerName, string gameName)
	{
		GD.Print(gameName + " created by " + peerName);

		_peer = new ENetMultiplayerPeer();
		Error err =  _peer.CreateServer(_port, maxClients:2);
		if (err != Error.Ok)
		{
			GD.Print("Error creating server");
			return;
		}

		_peer.Host.Compress(_compressionMode);
		Multiplayer.MultiplayerPeer = _peer;
		
		_playerRes = new PlayerRes(peerName, Multiplayer.GetUniqueId());
		AddPlayer(_playerRes.GetDict());
	}
	public void JoinGame(string peerName, string gameName)
	{
		GD.Print(peerName + " joined " + gameName);

		_peer = new ENetMultiplayerPeer();
		Error err =  _peer.CreateClient(_ipAddress, _port);

		if (err != Error.Ok)
		{
			GD.Print("Error joining server");
			return;
		}

		_peer.Host.Compress(_compressionMode);
		Multiplayer.MultiplayerPeer = _peer;
		_playerRes = new PlayerRes(peerName, Multiplayer.GetUniqueId());
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal=false, TransferMode=MultiplayerPeer.TransferModeEnum.Reliable)]
	public void AddPlayer(Godot.Collections.Dictionary<string, Variant> playerDict)
	{	
		PlayerRes playerRes = new PlayerRes(playerDict);
		if (!GlobalGameManager.PlayersResList.Contains(playerRes))
			GlobalGameManager.Add(playerRes);

		if (Multiplayer.IsServer())
		{
			foreach (PlayerRes res in GlobalGameManager.PlayersResList)
			{
				Rpc(MethodName.AddPlayer,res.GetDict());
			}

			if (GlobalGameManager.PlayersResList.Count == _minNumPeers)
				EmitSignal(SignalName.MinNumPeersConnected);
		}

		return;
	}
    
	
	private void OnConnectionFailed()
    {
		GD.Print("Connection failed");
    }
	private void OnPeerDisconnected(long id)
    {
		GD.Print("Peer disconnected:" + id);
    }
    private void OnPeerConnected(long id)
    {
		GD.Print("Peer connected:" + id);
    }	
	private void OnConnectedToServer()
	{
		GD.Print("Connected to server"); 
		RpcId(1, MethodName.AddPlayer, _playerRes.GetDict());
	}
	private void OnDisconnectedFromServer()
	{
		GD.Print("Disconnected from server");
	}
	
}
