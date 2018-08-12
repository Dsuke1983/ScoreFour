using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CpuGameController : MonoBehaviour {

	// orderを実際に変更するとゲームの手番が変わってしまうので、仮のorderの変数。
	private PieceType pieceTypeForCpuLogic = PieceType.None;

	// GameControllerを参照するために
	private GameController gameController;

	// Playerのコマの色
	private PieceType playerPieceType;

	// CPUのコマの色
	private PieceType cpuPieceType;

	// CPUのレベル1〜3
	private int cpuLevel;

	// CPUの思考回数
	private int cpuThoughtNumber = 100;

	// Use this for initialization
	void Start () {

		// GameComtrollerを参照する。
		// GameControllerとCpuGameControllerは同じオブジェクトにアタッチしなければならない。
		gameController = GetComponent <GameController> ();

		// CPUのコマをランダムに設定
		int intCpuPieceType = UnityEngine.Random.Range(3,4);
		cpuPieceType = (PieceType)intCpuPieceType;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CpuLogic(){

		// 現在の状況をコピーする
		Piece[, ,] pieceArrayNow = new Piece[GameController.pieceXCount, GameController.pieceYCount, GameController.pieceZCount];

		Array.Copy (gameController.pieceArray, pieceArrayNow, gameController.pieceArray.Length);

		// 勝てばポイントが増える三次元配列
		int[, ,] piecePoints = new int[GameController.pieceXCount, GameController.pieceYCount, GameController.pieceZCount];

		pieceTypeForCpuLogic = gameController.order;

		// 置けるおころがなくなるまで処理を繰り返す
		// ここでは1回だけ呼び出しているため、複数回シミュレーションする場合はforなどで繰り返すこと
		while (true) {

			// CPUがコマを置ける場所を取得
			List<Piece> putableList = getPutableList (pieceArrayNow);

			// 置けるところがなくなったら終了
			if (putableList.Count == 0) {
				break;
			}

			// 置ける場所のリストのランダムなIndexを取得
			int randomIndex = UnityEngine.Random.Range (0, putableList.Count - 1);
			Piece putablePiece = putableList [randomIndex];

			// コマの情報を更新
			putablePiece.SetPieceType (pieceTypeForCpuLogic);

			// ここは視覚的にわかりやすいようにオブジェクトを表示させています
			// 実際のシミュレーションでは削除すること
			PieceController controll = gameController.pieceObjArray [putablePiece.xIndex, putablePiece.yIndex, putablePiece.zIndex].GetComponent<PieceController> ();
			controll.gameObject.SetActive (true);
			controll.ShowPiece ();
			controll.ChangeMaterial (pieceTypeForCpuLogic);

			// ターンを変更する
			if (pieceTypeForCpuLogic == PieceType.Black) {
				pieceTypeForCpuLogic = PieceType.White;
			} else if (pieceTypeForCpuLogic == PieceType.White) {
				pieceTypeForCpuLogic = PieceType.Black;
			}
		}
	}

	// コマを置ける場所のリストを返す
	List<Piece> getPutableList(Piece[, ,] pieceArrayNow) {
		List<Piece> putableList = new List<Piece> ();

		for (int z = 0; z < 4; z++) {

			for (int x = 0; x < 4; x++) {

				for (int y = 0; y < 4; y++) {

					if (pieceArrayNow [x, y, z].pieceType == PieceType.None) {

						// 何も置かれていない場合は置ける場所のリストに加える
						putableList.Add (pieceArrayNow[x, y, z]);

						break;

					}
				}
			}
		}

		return putableList;

		// ゲーム難易度に応じて、どこに置くかを決める

	}

}
