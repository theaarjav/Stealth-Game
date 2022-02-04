using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class gameUI : MonoBehaviour {
	public GameObject GameLoseUI;
	public GameObject GameWinUI;
	bool GameIsOver;
	// Use this for initialization
	void Start () {
		guard.onPlayerSpotted += ShowGameLoseUI;
		FindObjectOfType<Player> ().OnFinish += ShowGameWinUI;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameIsOver) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				SceneManager.LoadScene (0);
			}
		}
	}

	void ShowGameWinUI(){
		GameOver (GameWinUI);
	}

	void ShowGameLoseUI(){
		GameOver (GameLoseUI);
	}

	void GameOver(GameObject gameoverUI){
		gameoverUI.SetActive (true);
		GameIsOver = true;
		guard.onPlayerSpotted -= ShowGameLoseUI;
		FindObjectOfType<Player> ().OnFinish -= ShowGameWinUI;
	}
}
