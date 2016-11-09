using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public enum GameMode {
	PLAY = 0,
	VIEW = 1
}

public class Game : MonoBehaviour {

	public static GameMode Mode;

	public float spawnPeriod = 1.0f;

	public float spawnAcceleration = 1.1f;

	public float accelerationPeriod = 30.0f;

	public float asteroidVelocity = 10f;

	public float asteroidAcceleration = 1.1f;

	public float asteroidMinAngle = -135;
	public float asteroidMaxAngle = -45;

	public float asteroidMinScale = 0.5f;
	public float asteroidMaxScale = 0.7f;

	public Rect spawnRect;

	private float lastSpawnTime = 0f; 

	private float lastAccelerationTime = 0f;

	public SpaceshipController spaceship;
    
	public Text scoreText;
	public GameObject pauseBtn;
	public GameObject pauseMenu;
	public GameObject finishMenu;

	public GameStateMessage gameState;

	// Use this for initialization
	void Start () {
		
		RestartGame();
	}
	
	// Update is called once per frame
	void Update () {
		if (Mode == GameMode.PLAY){
			UpdatePlayMode();
		} 
	}

	void UpdatePlayMode(){
		if (isGameStarted && !isPause){
			float t = GameTimer.instance.GetGameTime();
			if (t - lastAccelerationTime >= accelerationPeriod){
				lastAccelerationTime = t;
				AccelerateSpawn();
			}
			if (t - lastSpawnTime >= spawnPeriod){
				lastSpawnTime = t;
				SpawnAsteroid();
				//Debug.Log("lastSpawnTime "+lastSpawnTime);
			}
		}
	}

	IEnumerator SendState(){
		while (isGameStarted) {
			gameState = new GameStateMessage();
			gameState.gameTime = GameTimer.instance.GetGameTime();
			gameState.score = score;
			gameState.isPause = isPause;
			gameState.playerDirection = spaceship.direction;
			gameState.objects = new AsteroidState[PoolManager.instance.ObjectsCount];
			for (int i = 0; i < PoolManager.instance.ObjectsCount; i++){
				AsteroidController asteroid = PoolManager.instance.GetObjectByID(i) as AsteroidController;
				gameState.objects[i] = asteroid.state;
			}
			MultiplayerManager.instance.SendGameStateMessge(gameState);

			yield return new WaitForSeconds(0.1f);
		}
	}

	void OnGameState(NetworkMessage message){
		GameStateMessage newState = message.ReadMessage<GameStateMessage>();
		if (gameState == null || newState.gameTime >= gameState.gameTime){
			gameState = newState;
			score = gameState.score;
			Time.timeScale = gameState.isPause?0:1;
			spaceship.nextDirection = gameState.playerDirection;
			AsteroidController asteroid;
			for (int i = 0; i < gameState.objects.Length; i++){
				asteroid = PoolManager.instance.GetObjectByID(i) as AsteroidController;
				asteroid.state = gameState.objects[i];
			}
		}
	}


	private int _score = 0;
	private int score{
		get {
			return _score;
		}
		set {
			_score = value;
			scoreText.text = "Счёт: "+_score;
		}
	}

	private bool isGameStarted = false;

	private bool isPause = false;

	public void OnPauseClick(){
		isPause = !isPause;
		GameTimer.instance.pause = isPause;
		if (isPause){
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
			lastAccelerationTime += GameTimer.instance.pauseTime;
			lastSpawnTime += GameTimer.instance.pauseTime;
		}
		pauseBtn.SetActive(!isPause);
		pauseMenu.SetActive(isPause);
	}

	public void StartGame(){
		isGameStarted = true;
		isPause = false;
		score = 0;
		Time.timeScale = 1;
		GameTimer.instance.StartGame(Time.realtimeSinceStartup);
		lastAccelerationTime = lastSpawnTime = GameTimer.instance.GetGameTime();
		pauseBtn.SetActive(true);
		pauseMenu.SetActive(false);
		finishMenu.SetActive(false);

		if (Mode == GameMode.VIEW){
			MultiplayerManager.instance.RegisterGameStateMessageHandler(OnGameState);
		} else if (Mode == GameMode.PLAY){
			StartCoroutine(SendState());
		}

	}
	private void StopGame(){
		Time.timeScale = 0;
		isGameStarted = false;
		GameTimer.instance.StopGame();
		if (Mode == GameMode.VIEW){
			MultiplayerManager.instance.UnregisterGameStateMessageHandler();
		}
	}

	public void RestartGame(){
		FinishGame();
		StartGame();
	}

	public void ExitToMenu(){
		FinishGame();
		SceneManager.LoadScene("Menu");
	}

	public void ShowFinish(){
		pauseBtn.SetActive(false);
		finishMenu.SetActive(true);
		StopGame();
	}

	private void FinishGame(){
		StopGame();
		PoolManager.instance.ReleaseAllObjects();
	}

	private void AccelerateSpawn(){
		spawnPeriod /= spawnAcceleration;
		asteroidVelocity *= asteroidAcceleration;
	}

	private void SpawnAsteroid(){
		Vector3 position = new Vector3(Random.Range(spawnRect.xMin, spawnRect.xMax),Random.Range(spawnRect.yMin, spawnRect.yMax),0);
		AsteroidController asteroid = PoolManager.instance.GetFreeObject(position) as AsteroidController;
		if (asteroid != null){
			float angle = Random.Range(asteroidMinAngle, asteroidMaxAngle) * Mathf.PI / 180;
			Vector3 velocity = new Vector3(Mathf.Cos(angle)*asteroidVelocity,Mathf.Sin(angle)*asteroidVelocity,0);
			float scale = Random.Range(asteroidMinScale, asteroidMaxScale);
			asteroid.Init(velocity, scale);
			asteroid.OnAsteroidCollect = OnAsteroidCollect;
			asteroid.OnAsteroidStrike = OnAsteroidStrike;
		}
	}

	private void OnAsteroidCollect(AsteroidController asteroid){
		score++;
		PoolManager.instance.ReleaseObject(asteroid as PoolObject);
	}

	private void OnAsteroidStrike(AsteroidController asteroid){
		PoolManager.instance.ReleaseObject(asteroid as PoolObject);
		ShowFinish();
	}

}
