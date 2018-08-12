using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			Application.LoadLevel ("2pOfflineGameScene");
			break;

		case "cpuLevel1":
			gameMode = GameMode.CpuLevel1;
			Application.LoadLevel ("CpuGameScene");
			break;

		case "cpuLevel2":
			gameMode = GameMode.CpuLevel2;
			Application.LoadLevel ("CpuGameScene");
			break;

		case "cpuLevel3":
			gameMode = GameMode.CpuLevel2;
			Application.LoadLevel ("CpuGameScene");
			break;

		default:
			break;
		}
	}
}
