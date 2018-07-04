using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	GameObject targetObj;
	Vector3 targetPos;

	void Start () {
		targetObj = GameObject.Find("Orign");
		targetPos = targetObj.transform.position;
	}

	void Update() {
		if (Input.GetMouseButton(1)) {
			// マウスの移動量
			float mouseInputX = Input.GetAxis("Mouse X");
			float mouseInputY = Input.GetAxis("Mouse Y");

			Vector3 objRotation = transform.rotation.eulerAngles;

			// 回転量
			float rotateDegrees =  -mouseInputY * Time.deltaTime * 200f;

			// 90度以上にならない様に制御している
			if (objRotation.x + rotateDegrees > 90) {
				rotateDegrees = 90 - objRotation.x;
			}

			// 0度未満にならない様に制御
			if (objRotation.x + rotateDegrees < 0) {
				rotateDegrees = - objRotation.x;
			}

			// targetの位置のY軸を中心に、回転（公転）する
			transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * 200f);
			// カメラの垂直移動
			transform.RotateAround(targetPos, transform.right, rotateDegrees);
		}
	}  
}
