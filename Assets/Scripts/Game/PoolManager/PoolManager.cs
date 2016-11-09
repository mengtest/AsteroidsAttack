using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour {

	private static PoolManager _instance = null;
	public static PoolManager instance {
		get{
			return _instance;
		}
	}

	void Awake(){
		if (_instance == null){
			InitPool();
			_instance = this;

		} else if (_instance != this){
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}
	// used objects list
	private List<PoolObject> usedObjects = new List<PoolObject>();
	// free objects list
	private List<PoolObject> freeObjects = new List<PoolObject>();
	// all objects list
	private List<PoolObject> objects = new List<PoolObject>();

	// prefab to create objects from
	public GameObject prefab = null;

	// objects count to create on init
	public int ObjectsCountOnStart = 10;

	// current objects count in pool
	public int ObjectsCount {
		get {
			return objects.Count;
		}
	}
	// initial create objects in pool
	private void InitPool(){
		PoolObject poolObject;
		for(int i = 0; i < ObjectsCountOnStart; i++){
			poolObject = CreateNewObject(Vector3.zero);
			ReleaseObject(poolObject);
		}
	}
	// get free object or create new
	public PoolObject GetFreeObject(Vector3 position){
		PoolObject poolObject;

		if (freeObjects.Count > 0){
			// use existing object
			poolObject = freeObjects[0];
			freeObjects.RemoveAt(0);
		} else {
			// create new object
			poolObject = CreateNewObject(position);
		}
		poolObject.InitPoolObject(position);
		usedObjects.Add(poolObject);

		return poolObject;
	}
	// transform used object to free
	public void SetObjectUsed(PoolObject poolObject, Vector3 position){
		if (freeObjects.Contains(poolObject)){
			freeObjects.Remove(poolObject);
		}
		poolObject.InitPoolObject(position);
		usedObjects.Add(poolObject);
	}
	// create new object from prefab
	private PoolObject CreateNewObject(Vector3 position){
		PoolObject poolObject;
		if (prefab != null){
			GameObject go = Instantiate(prefab, position, Quaternion.identity) as GameObject;
			DontDestroyOnLoad(go);
			poolObject = go.GetComponent<PoolObject>();
			poolObject.objectId = objects.Count;
			objects.Add(poolObject);
			return poolObject;	
		}
		return null;
	}
	// transform all objects to free
	public void ReleaseAllObjects(){
		while (usedObjects.Count > 0){
			ReleaseObject(usedObjects[0]);
		}
	}
	// transform used object to free
	public void ReleaseObject(PoolObject poolObject){
		if (usedObjects.Contains(poolObject)){
			usedObjects.Remove(poolObject);
		}
		poolObject.ReleasePoolObject();
		freeObjects.Add(poolObject);

	}
	// get objects by id (id == index)
	public PoolObject GetObjectByID(int objectID){
		if (objectID >= 0){
			while (objectID >= objects.Count){
				PoolObject poolObject = CreateNewObject(Vector3.zero);
				ReleaseObject(poolObject);
			}
			return objects[objectID];
		}
		return null;
	}

}
