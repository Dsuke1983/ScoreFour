using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour {

	// ゲームモード
	public static GameMode gameMode;

	// CpuGameController
	private CpuGameController cpuGameContloller;

	// スタートボタン
	public GameObject startButton;


	// Use this for initialization
	void Start () {

		cpuGameContloller = GetComponent <CpuGameController> ();

		startButton = GameObject.Find("start");
	
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
			CpuGameController.cpuLevel = 1;
			SceneManager.LoadScene ("CpuGameScene");
			break;

		case "cpuLevel2":
			gameMode = GameMode.CpuLevel2;
			CpuGameController.cpuLevel = 2;
			SceneManager.LoadScene ("CpuGameScene");
			break;

		case "cpuLevel3":
			gameMode = GameMode.CpuLevel3;
			CpuGameController.cpuLevel = 3;
			SceneManager.LoadScene ("CpuGameScene");
			break;

		case "start":
			//cpuGameContloller.CpuStartButton ();
			startButton.gameObject.SetActive (false);
			break;

		case "back":
			SceneManager.LoadScene ("Opening");
			break;

		default:
			break;
		}
	}
}
