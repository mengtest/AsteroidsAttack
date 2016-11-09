using UnityEngine;
using System.Collections;

public class GameTimer  {

	private static GameTimer _instance = null;
	public static GameTimer instance{
		get {
			if (_instance == null){
				_instance = new GameTimer();
			}
			return _instance;
		}
	}

	protected GameTimer(){

	}

	private float localTimeOffset = 0f;
	private float gameTimeOffset = 0f;
	private bool isStarted = false;

	public void StartGame(float gameStartTime){
		localTimeOffset = Time.realtimeSinceStartup;
		gameTimeOffset = gameStartTime;
		isStarted = true;
		_pause = false;
		startPauseTime = 0f;
		stopPauseTime = 0f;
	}
	public void StopGame(){
		isStarted = false;	
	}

	public float GetGameTime(){
		if (isStarted){
			return gameTimeOffset + (Time.realtimeSinceStartup - localTimeOffset);
		}
		return 0;
	}

	private float startPauseTime = 0f;
	private float stopPauseTime = 0f;

	private bool _pause = false;
	public bool pause {
		get {
			return _pause;
		}
		set {
			_pause = value;
			if (_pause){
				startPauseTime = GetGameTime();
			} else {
				stopPauseTime = GetGameTime();
			}
		}
	}


	public float pauseTime{
		get {
			return stopPauseTime - startPauseTime;
		}
	}
}
