using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

	private int xDimension = 0;
	private int yDimension = 0;
	private int zDimension = 0;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void SetDimension(int x, int y, int z) {

		xDimension = x;
		yDimension = y;
		zDimension = z;

	}

	public int GetDimensionX() {

		return xDimension;

	}

	public int GetDimensionY() {

		return yDimension;

	}

	public int GetDimensionZ() {

		return zDimension;

	}

	public void ShowPiece() {

		gameObject.GetComponent<MeshRenderer> ().enabled = true;

	}

}
