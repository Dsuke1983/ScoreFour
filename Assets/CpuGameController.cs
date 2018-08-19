using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PiecePoints {

	public int pointXindex;
	public int pointYindex;
	public int pointZindex;
	public int cpuPiecePoints;
	public int cpuPiecePointsRanking;

	public PiecePoints (int pointXindex, int pointYindex, int pointZindex, int cpuPiecePoints, int cpuPiecePointsRanking){
		this.pointXindex = pointXindex;
		this.pointYindex = pointYindex;
		this.pointZindex = pointZindex;
		this.cpuPiecePoints = cpuPiecePoints;
		this.cpuPiecePointsRanking = cpuPiecePointsRanking;
	}
}

public class CpuGameController : MonoBehaviour {

	// orderを実際に変更するとゲームの手番が変わってしまうので、仮のorderの変数。
	private PieceType pieceTypeForCpuLogic = PieceType.None;

	// GameControllerを参照するために
	private GameController gameController;

	// Playerのコマの色
	private PieceType playerPieceType;

	// CPUのコマの色
	public static PieceType cpuPieceType;

	// CPUのレベル1〜3
	private int cpuLevel;

	// CPUの思考回数
	private int cpuThoughtNumber = 100;

	// 勝てばポイントが増える三次元配列
	public PiecePoints[, ,] piecePointsArray = new PiecePoints[GameController.pieceXCount, GameController.pieceYCount, GameController.pieceZCount];


	// シミュレーション用
	Piece[, ,] pieceArrayNow;

	// Use this for initialization
	void Start () {

		// GameComtrollerを参照する。
		// GameControllerとCpuGameControllerは同じオブジェクトにアタッチしなければならない。
		gameController = GetComponent <GameController> ();

		// 先攻後攻のテキスト
		GameObject senkouText = GameObject.Find ("Senkou");
		GameObject koukouText = GameObject.Find ("Koukou");

		// CPUのコマをランダムに設定
		int intCpuPieceType = UnityEngine.Random.Range(3,5);
		cpuPieceType = (PieceType)intCpuPieceType;

		// 先攻後攻のテキストを表示
		if (cpuPieceType == PieceType.Black) {
			senkouText.GetComponent<Text> ().text = "CPU";
			koukouText.GetComponent<Text> ().text = "Player";
			playerPieceType = PieceType.White;
			CpuLogic ();
		} else if (cpuPieceType == PieceType.White) {
			senkouText.GetComponent<Text> ().text = "Player";
			koukouText.GetComponent<Text> ().text = "CPU";
			playerPieceType = PieceType.Black;
		}

		// PiecePointsArrayの中身を初期化
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				for (int z = 0; z < 4; z++) {

					// コマの情報を設定
					piecePointsArray [x, y, z] = new PiecePoints (x, y, z, 0, 0);

				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CpuLogic(){

		// 現在の状況をコピーする
		pieceArrayNow = new Piece[GameController.pieceXCount, GameController.pieceYCount, GameController.pieceZCount];

		Array.Copy (gameController.pieceArray, pieceArrayNow, gameController.pieceArray.Length);

		//piecePointsを全て0にする
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				for (int z = 0; z < 4; z++) {

					piecePointsArray [x, y, z].cpuPiecePoints = 0;

				}
			}
		}

		// 最初、CPUのPieceTypeを取得し、シミュレーションで手番を入れ替える変数
		pieceTypeForCpuLogic = gameController.order;

		// 次の一手を置ける場所を取得(実際に置く場所の候補)
		List<Piece> firstPutableList = getPutableList (pieceArrayNow);

		// もし置けるところがなかった場合、終了(ゲームが終了しているはずなので基本的にはないはず)
		if (firstPutableList.Count == 0) {
			return;
		}

		// リストから抽出して1つ目の位置を順番に処理していく(最大16箇所)
		for (int i = 0; i < firstPutableList.Count; i++){

			// PieceArrayをPieceArrayNowにコピーして、現在の状況と一致させる
			Array.Copy (gameController.pieceArray, pieceArrayNow, gameController.pieceArray.Length);

			Piece firstPutablePiece = firstPutableList [i];

			// PieceArrayNowから1つ目に置くコマを選択
			pieceArrayNow[firstPutablePiece.xIndex, firstPutablePiece.yIndex, firstPutablePiece.zIndex].SetPieceType (pieceTypeForCpuLogic);

			// 4つ揃っているか判定して揃ってた場合、piecePointsに+1する
			// 0 = NONE / 1 = 揃っている
			if (gameController.GameCheck (pieceArrayNow) == 1) {
				piecePointsArray [firstPutablePiece.xIndex, firstPutablePiece.yIndex, firstPutablePiece.zIndex].cpuPiecePoints++;
				continue;
			}

			// ターンを変更する
			if (pieceTypeForCpuLogic == PieceType.Black) {
				pieceTypeForCpuLogic = PieceType.White;
			} else if (pieceTypeForCpuLogic == PieceType.White) {
				pieceTypeForCpuLogic = PieceType.Black;
			}

			// 2個目以降はランダム
			// CpuSimulationはPlayerターンからスタートする
			// CpuSimulation戻り値いるくね？
			CpuSimulation (firstPutablePiece.xIndex, firstPutablePiece.yIndex, firstPutablePiece.zIndex);

		}

		// 勝率が100%のところがあった場合、CPUレベルに関係なく置いて関数を抜ける
		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 4; y++) {
				for (int z = 0; z < 4; z++) {
					if (piecePointsArray [x, y, z].cpuPiecePoints == cpuThoughtNumber) {

						putPiece (cpuPieceType, x, y, z);

						return;
					}
				}
			}
		}

		// Listを作ってListに入れ、ソート
		List<PiecePoints> piecePointsList = new List<PiecePoints> ();

		for (int z = 0; z < 4; z++) {

			for (int x = 0; x < 4; x++) {

				for (int y = 0; y < 4; y++) {

					piecePointsList.Add (piecePointsArray[x, y, z]);

				}
			}
		}
		piecePointsList.Sort((a, b) => b.cpuPiecePoints - a.cpuPiecePoints);


		// CPUのレベルに応じて置く場所を決定
		switch (cpuLevel) {

		case 1:
			putPiece (cpuPieceType, piecePointsList[2].pointXindex, piecePointsList[2].pointYindex, piecePointsList[2].pointZindex);
			break;

		case 2:
			putPiece (cpuPieceType, piecePointsList[1].pointXindex, piecePointsList[1].pointYindex, piecePointsList[1].pointZindex);
			break;

		case 3:
			putPiece (cpuPieceType, piecePointsList[0].pointXindex, piecePointsList[0].pointYindex, piecePointsList[0].pointZindex);
			break;

		default:
			Debug.Log ("エラー");		
			break;

		}

		// 実際に置く
		// ここよくわからん
		// PieceController controll = pieceObjArray [putablePiece.xIndex, putablePiece.yIndex, putablePiece.zIndex].GetComponent<PieceController>();
		// controll.gameObject.SetActive(true);
		// controll.ShowPiece ();
		// controll.ChangeMaterial (pieceTypeForCpuLogic);
	}

	void CpuSimulation(int pointsX, int pointsY, int pointsZ) {

		bool cpuSimulationRoopFlug;

		for (int i = 0; i < cpuThoughtNumber; i++) {

			cpuSimulationRoopFlug = true;

			while (cpuSimulationRoopFlug) {
				
				// コマを置ける場所(PieceTypeがNoneの場所)を取得
				List<Piece> putableList = getPutableList (pieceArrayNow);

				// もし置けるところがなかった場合、終了(ゲームが終了しているはずなので基本的にはないはず)
				if (putableList.Count == 0) {
					return;
				}

				// 置ける場所のリストのランダムなIndexを取得
				// 置ける場所からランダムに1つ選ぶ
				int randomIndex = UnityEngine.Random.Range (0, putableList.Count);
				Piece putablePiece = putableList [randomIndex];

				// コマの情報を更新
				pieceArrayNow [putablePiece.xIndex, putablePiece.yIndex, putablePiece.zIndex].SetPieceType (pieceTypeForCpuLogic);

				// ここは視覚的にわかりやすいようにオブジェクトを表示させています
				// 実際のシミュレーションでは削除すること
				// PieceController controll = gameController.pieceObjArray [putablePiece.xIndex, putablePiece.yIndex, putablePiece.zIndex].GetComponent<PieceController> ();
				// controll.gameObject.SetActive (true);
				// controll.ShowPiece ();
				// controll.ChangeMaterial (pieceTypeForCpuLogic);

				// 4つ揃っているか判定して揃ってた場合、piecePointsに+1する
				if (gameController.GameCheck (pieceArrayNow) == 0) {
					// ターンを変更する
					if (pieceTypeForCpuLogic == PieceType.Black) {
						pieceTypeForCpuLogic = PieceType.White;
					} else if (pieceTypeForCpuLogic == PieceType.White) {
						pieceTypeForCpuLogic = PieceType.Black;
					}
				} else if (gameController.GameCheck (pieceArrayNow) == 1) {
					// 勝ったのがcpuPieceTypeなら、piecePointsに1プラスする
					if (pieceTypeForCpuLogic == cpuPieceType) {
						piecePointsArray[pointsX, pointsY, pointsZ].cpuPiecePoints++;
					}
					cpuSimulationRoopFlug = false;
					break;
				} else if (gameController.GameCheck (pieceArrayNow) == 3) {
					// 引き分けだった場合、なにもしない
					cpuSimulationRoopFlug = false;
					break;
				}
			}
		}

	}
	// コマを置ける場所のリストを返す
	// 置ける場所のPieceクラスの情報がリストで入っている
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

	}

	// 実際にコマを置く
	public void putPiece(PieceType pieceType, int x, int y, int z){

		PieceController pieceController = gameController.pieceObjArray [x, y, z].GetComponent<PieceController> ();
		Piece piece = gameController.pieceArray [x, y, z];

		pieceController.gameObject.SetActive (true);
		pieceController.ShowPiece ();
		pieceController.ChangeMaterial (pieceType);
		piece.SetPieceType (pieceType);
		MyAudio.put2SoundFlug = true;

	}
}
