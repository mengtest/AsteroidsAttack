using UnityEngine;
using System.Collections;

public class AsteroidController : PoolObject {

	private AsteroidState _state = new AsteroidState(Vector3.zero);
	public AsteroidState state {
		get {
			_state.position = transform.position;
			_state.scale = transform.localScale;
			_state.isUsed = gameObject.activeSelf;
			_state.isCollected = isCollected;
			_state.velocity = GetComponent<Rigidbody>().velocity.magnitude;
			return _state;
		}
		set{
			_state = value;
			transform.localScale = _state.scale;

			RefreshPosition();
		}
	}

	private bool isMovingToStatePosition = false;

	private void RefreshPosition(){
		if (_state.isUsed && !gameObject.activeSelf){
			// use object
			PoolManager.instance.SetObjectUsed(this, _state.position);
		}
		if (!_state.isUsed && gameObject.activeSelf){
			// free object
			PoolManager.instance.ReleaseObject(this as PoolObject);
		} 
		if (_state.isCollected && !isCollected){
			// collect object
			CollectAsteroid();
		}
		// start moving to new position
		if (!_state.position.Equals (transform.position)){
			if (_state.position.y > transform.position.y){
				transform.position = _state.position;
				isMovingToStatePosition = false;
			} else {
				isMovingToStatePosition = true;
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	private Vector3 translation;

	// Update is called once per frame
	void Update () {

		if (Game.Mode == GameMode.VIEW){
			// moving to state position
			if (isMovingToStatePosition){
				Vector3 direction = _state.position - transform.position;
				transform.Translate(direction.normalized * _state.velocity * Time.deltaTime);
			}

		}
		// free flied away objects
		if (gameObject.transform.position.y < -10 && !isCollected){
			PoolManager.instance.ReleaseObject(this as PoolObject);
		}

	}

	private GameObject explosion;
	private UVSpriteAnimation anim;

	public void Init(Vector3 velocity, float scale){
		transform.localScale = new Vector3(scale, scale, scale);
		gameObject.GetComponent<Rigidbody>().velocity = velocity;
	}

	public override void InitPoolObject(Vector3 position){
		base.InitPoolObject(position);
		isCollected = false;
		anim = gameObject.GetComponent<UVSpriteAnimation>();
		anim.StartAnimation(0);
		initExplosion();
		gameObject.GetComponent<Rigidbody>().isKinematic = (Game.Mode == GameMode.VIEW);
	}

	public override void ReleasePoolObject(){
		base.ReleasePoolObject();
		anim = gameObject.GetComponent<UVSpriteAnimation>();
		anim.StopAnimation();
		initExplosion();
		isMovingToStatePosition = false;
		OnAsteroidCollect = null;
		OnAsteroidStrike = null;
	}

	private void initExplosion(){
		explosion = gameObject.transform.GetChild(0).gameObject;
		explosion.SetActive(false);

	}

	public delegate void AsteroidCollect(AsteroidController asteroid);
	public AsteroidCollect OnAsteroidCollect;

	public delegate void AsteroidStrike(AsteroidController asteroid);
	public AsteroidStrike OnAsteroidStrike;


	void OnTriggerEnter(Collider other){
		CheckCollision(other.tag);
	}
	void OnTriggerStay(Collider other){
		CheckCollision(other.tag);
	}

	private void CheckCollision(string tag){
		
		if (Game.Mode == GameMode.PLAY && !isCollected){
			if (tag == "Player"){
				CollectAsteroid();
			}
			if (tag == "Earth"){
				EarthStrike();
			}
		}
	}

	private bool isCollected = false;

	private void CollectAsteroid(){
		anim.StopAnimation();
		explosion.SetActive(true);
		explosion.GetComponent<UVSpriteAnimation>().StartAnimation(0);
		isCollected = true;
		if (Game.Mode == GameMode.PLAY){
			Invoke("Collect", 0.3f);
		}
	}

	private void Collect(){

		OnAsteroidCollect(this);
	}

	private void EarthStrike(){
		anim.StopAnimation();
		explosion.SetActive(true);
		explosion.GetComponent<UVSpriteAnimation>().StartAnimation(0);
		isCollected = true;
		if (Game.Mode == GameMode.PLAY){
			Invoke("GameOver", 0.1f);
		}

	}
	private void GameOver(){

		OnAsteroidStrike(this);
	}
}
