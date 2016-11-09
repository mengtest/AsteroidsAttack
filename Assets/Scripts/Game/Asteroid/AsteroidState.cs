using UnityEngine;
using System.Collections;

public struct AsteroidState  {

	public Vector3 position;
	public Vector3 scale;
	public float velocity;
	public bool isUsed;
	public bool isCollected;

	public AsteroidState(Vector3 position){
		this.position = position;
		scale = new Vector3(1, 1, 1);
		velocity = 0;
		isUsed = false;
		isCollected = false;
	}

}
