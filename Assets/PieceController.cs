using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

	private int xDimension = 0;
	private int yDimension = 0;
	private int zDimension = 0;

	public Material materialBlack;
	public Material materialWhite;
	public Material materialRed;


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

	public void ChangeMaterial(PieceType order) {

		if (order == PieceType.Black) {

			this.gameObject.GetComponent<Renderer> ().material = materialBlack;

		} else if (order == PieceType.White) {

			this.gameObject.GetComponent<Renderer> ().material = materialWhite;

		} else if (order == PieceType.Red) {

			this.gameObject.GetComponent<Renderer> ().material = materialRed;

		}
	}
}
