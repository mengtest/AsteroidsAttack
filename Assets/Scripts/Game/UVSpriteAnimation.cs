using UnityEngine;
using System.Collections;

public class UVSpriteAnimation : MonoBehaviour {

	public bool autoStart = false;
	public bool loop = true;

	public int cols = 8;
	public int rows = 8;
	public float fps = 30.0f;

	private int index = 0;

	private bool isStarted = false;

	private Material material = null;
	private string textureName = "_MainTex";

	public int FirstFrameIndex = 0;
	public int LastFrameIndex = 30;


	// Use this for initialization
	void Start () {
		
		if (autoStart){
			StartAnimation(index);
		}
	}

	public void StartAnimation(int startIndex){
		isStarted = true;
		index = startIndex;

		material = GetComponent<Renderer>().materials[0];
		Vector2 size = new Vector2(1f/cols, 1f/ rows);
		material.SetTextureScale(textureName, size);

		StartCoroutine(UpdateFrame());
	}



	public void StopAnimation(){
		isStarted = false;
		material = GetComponent<Renderer>().materials[0];
		material.SetTextureOffset(textureName, new Vector2(1f, 0f));
	}


	private IEnumerator UpdateFrame(){
		while (isStarted){
			NextFrame();
			yield return new WaitForSeconds(1f/fps);
		}
	}

	private void NextFrame(){
		index++;
		if (index > LastFrameIndex){
			if (loop){
				index = FirstFrameIndex;
			} else {
				StopAnimation();
			}

		}

		if (isStarted){
			Vector2 offset = new Vector2((float)(index % cols)/ (float) cols, 1f - (float)((index / cols) + 1f) / (float) rows);
			material.SetTextureOffset(textureName, offset);
		}
	}


}
