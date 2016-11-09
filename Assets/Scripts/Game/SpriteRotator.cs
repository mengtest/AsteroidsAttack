using UnityEngine;
using System.Collections;

public class SpriteRotator : MonoBehaviour {

	public Vector3 rotation = new Vector3(0, 0, 1);

	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {
		transform.Rotate(rotation * Time.deltaTime);
	}
}
