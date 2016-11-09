using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class GameStateMessage : MessageBase {

	public float playerDirection = 90;
	public int score = 0;
	public bool isGameStarted = false;
	public bool isPause = false;
	public float gameTime = 0;
	public AsteroidState[] objects;

}
