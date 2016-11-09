using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public GameObject mainMenu;
	public GameObject configMenu;
	public Text StatusBar;
	public InputField IPText;
	// Use this for initialization


	void Start () {
		ShowMainMenu();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void StartGamePlay(){
		Game.Mode = GameMode.PLAY;
		SceneManager.LoadScene("Game");
	}

	public void ShowConfig(){
		mainMenu.SetActive(false);
		configMenu.SetActive(true);
	}

	public void ShowMainMenu(){
		mainMenu.SetActive(true);
		configMenu.SetActive(false);

		StatusBar.text = "IP=" + MultiplayerManager.instance.deviceIP;
	}

	public void StartGameView(){
		
		MultiplayerManager.instance.StartClient(IPText.text);
		Game.Mode = GameMode.VIEW;
		SceneManager.LoadScene("Game");
	}
	public void ExitGame(){
		Application.Quit();
	}
}
