using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameMsgType {
	public static short GameState = MsgType.Highest + 1;
};

public class MultiplayerManager : MonoBehaviour {

	private static MultiplayerManager _instance = null;
	public static MultiplayerManager instance {
		get{
			return _instance;
		}
	}

	void Awake(){
		if (_instance == null){
			_instance = this;
			Init();
		} else if (_instance != this){
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	public NetworkManager networkManager;

	public string deviceIP = "0.0.0.0";

	private void Init(){
		deviceIP = Network.player.ipAddress;
		networkManager.networkAddress = deviceIP;
		NetworkServer.Listen(deviceIP, networkManager.networkPort);
	}

	NetworkClient client;

	public void StartClient(string ip){
		client = new NetworkClient();
		client.RegisterHandler(MsgType.Connect, ConnectToServerHandler);
		client.RegisterHandler(MsgType.Disconnect, DisconnectFromServerHandler);
		client.RegisterHandler(MsgType.Error, ErrorHandler);
		client.Connect(ip, networkManager.networkPort);
	}

	public void UnregisterGameStateMessageHandler(){
		client.UnregisterHandler(GameMsgType.GameState);
	}

	public void RegisterGameStateMessageHandler(NetworkMessageDelegate netMesDelegate){
		client.RegisterHandler(GameMsgType.GameState, netMesDelegate);
	}

	private void ConnectToServerHandler(NetworkMessage message){
		Debug.Log("CONNECTED! " + message.ToString());
	}

	private void ErrorHandler(NetworkMessage message){
		Debug.Log("ERROR! " + message.ToString());
	}

	private void DisconnectFromServerHandler(NetworkMessage message){
		Debug.Log("DISCONNECTED! " + message.ToString());
	}

	public void SendGameStateMessge(GameStateMessage message){
		NetworkServer.SendToAll(GameMsgType.GameState, message);
	}

}

