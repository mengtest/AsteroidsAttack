using UnityEngine;
using System.Collections;

public class SpaceshipController : MonoBehaviour {

	public GameObject container;

	public Vector2 center = new Vector2(0f, -30f);

	public float radius = 33f;

	public float turnSpeed = 1f;

	public float direction = 90f;

	public float nextDirection = 90f;

	public float minDirection = 60f;

	public float maxDirection = 120f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Game.Mode == GameMode.PLAY){
			if (Input.GetKey(KeyCode.LeftArrow)){
				Move(Time.deltaTime);
			} else if (Input.GetKey(KeyCode.RightArrow)){
				Move(-Time.deltaTime);
			}

			if (Mathf.Abs(Input.acceleration.x) > 0.05f){
				Move(- Time.deltaTime * Input.acceleration.x * 3);
			} 
		} else if (Game.Mode == GameMode.VIEW){
			if (nextDirection < direction){
				MoveLeftToDirection(Time.deltaTime);	
			} else if (nextDirection > direction){
				MoveRightToDirection(Time.deltaTime);
			}
		}

		UpdatePosition();
	}
	private void MoveLeftToDirection(float t){
		direction -= turnSpeed * t;
		if (direction < nextDirection){
			direction = nextDirection;
		}
	}

	private void MoveRightToDirection(float t){
		direction += turnSpeed * t;
		if (direction > nextDirection){
			direction = nextDirection;
		}
	}

	private void Move(float t){
		direction += turnSpeed * t;
		if (direction > maxDirection){
			direction = maxDirection;
		}
		if (direction < minDirection){
			direction = minDirection;
		}
	}
		
	private Vector3 newPosition = new Vector3(0,0,0);
	private Vector3 newRotation = new Vector3(0,0,0);

	private void UpdatePosition(){
		newPosition.x = center.x + Mathf.Cos(direction*Mathf.PI/180) * radius;
		newPosition.y = center.y + Mathf.Sin(direction*Mathf.PI/180) * radius;
		newPosition.z = container.transform.position.z;
		container.transform.position = newPosition;

		newRotation.z = direction-90;
		container.transform.rotation = Quaternion.Euler(newRotation);
	}
}
