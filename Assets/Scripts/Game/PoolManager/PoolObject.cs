using UnityEngine;
using System.Collections;

public class PoolObject : MonoBehaviour {

	public int objectId = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void InitPoolObject(Vector3 position){
		gameObject.SetActive(true);
		gameObject.transform.position = position;

	}

	public virtual void ReleasePoolObject(){
		gameObject.SetActive(false);
	}
}
