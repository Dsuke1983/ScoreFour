using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDemo : MonoBehaviour {

	GameObject targetObj;
	Vector3 targetPos;

	void Start () {
		targetObj = GameObject.Find("Orign");
		targetPos = targetObj.transform.position;
	}

	void Update() {

		// targetの位置のY軸を中心に、回転（公転）する
		transform.RotateAround(targetPos, Vector3.up, -10f * Time.deltaTime);
	}  
}
