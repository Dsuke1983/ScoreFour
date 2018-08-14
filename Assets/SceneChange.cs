using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

	// ゲームモード
	public static GameMode gameMode;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ButtonClick() {

		switch (transform.name) {

		case "2players_online":
			gameMode = GameMode.Online2p;
			break;

		case "2players_offline":
			gameMode = GameMode.Offline2P;
			SceneManager.LoadScene ("2pOfflineGameScene");
			break;

		case "cpuLevel1":
			gameMode = GameMode.CpuLevel1;
			SceneManager.LoadScene ("CpuGameScene");
			break;

		case "cpuLevel2":
			gameMode = GameMode.CpuLevel2;
			SceneManager.LoadScene ("CpuGameScene");
			break;

		case "cpuLevel3":
			gameMode = GameMode.CpuLevel2;
			SceneManager.LoadScene ("CpuGameScene");
			break;

		case "back":
			SceneManager.LoadScene ("Opening");
			break;

		default:
			break;
		}
	}
}
