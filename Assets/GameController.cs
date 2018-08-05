using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PieceType {
	Initial,
	None,
	Red,
	Black,
	White
}

public class Piece {

	// 生成するポジションを入れるVector3
	public Vector3 position;

	// コマの種類を格納する変数
	public PieceType pieceType;

	// このクラスを初期化する時に呼ぶコンストラクタ
	public Piece(Vector3 position, PieceType pieceType) {
		this.position = position;
		this.pieceType = pieceType;
	}

	// コマの種類を設定する
	public void SetPieceType(PieceType pieceType) {
		this.pieceType = pieceType;
	}
}

public class GameController : MonoBehaviour {

	// Playerのコマの色
	PieceType playerPieceType;

	// CPUのコマの色
	PieceType cpuPieceType;

	// CPUのレベル1〜3
	int cpuLevel;

	// CPUの思考回数
	int cpuThoughtNumber = 100;

	// ゲームの勝敗を格納
	public static bool blackWin = false;
	public static bool whiteWin = false;

	// 生成するコマのPrefab
	public GameObject piecePrefab;

	// 今の手番を格納
	private PieceType order = PieceType.Black;

	// 最初にRayが当たったオブジェクトの情報を格納
	private PieceController selectedPieceController;

	// このレイヤーだけにRayによる当たり判定をさせる
	public LayerMask layerMask;

	// コマの数(定数)
	const int pieceXCount = 4;
	const int pieceYCount = 4;
	const int pieceZCount = 4;

	// コマの生成間隔
	const float xSpace = 1;
	const float ySpace = 1;
	const float zSpace = 1;

	// Pieceクラスの三次元配列
	private Piece[, ,] pieceArray = new Piece[pieceXCount, pieceYCount, pieceZCount];

	// 何の三次元配列？
	private GameObject[, ,] pieceObjArray = new GameObject[pieceXCount, pieceYCount, pieceZCount];

	void Start () {

		// CPUのコマをランダムに設定
		int intCpuPieceType = UnityEngine.Random.Range(3,4);

		cpuPieceType = (PieceType)intCpuPieceType;



		// 初期コマの設定
		for (int i = 0; i < pieceXCount; i++) {
			for (int j = 0; j < pieceYCount; j++) {
				for (int k = 0; k < pieceZCount; k++) {

					// コマの座標
					Vector3 piecePos = new Vector3 (i * xSpace, j * ySpace, k * zSpace);

					// コマの情報を設定
					pieceArray [i, j, k] = new Piece (piecePos, PieceType.None);

				}
			}
		}

		// 配列の1次元(X)
		for (int i = 0; i < pieceArray.GetLength (0); i++) {
			// 配列の2次元(Y)
			for (int j = 0; j < pieceArray.GetLength (1); j++) {
				// 配列の3次元(Z)
				for (int k = 0; k < pieceArray.GetLength (2); k++) {

					// GameObjectを生成
					GameObject pieceObj = Instantiate (piecePrefab) as GameObject;

					// 生成したコマのGameObjectのScriptにアクセスする為
					PieceController pieceController = pieceObj.GetComponent<PieceController> ();

					// コマにx,y,zの次元数を設定
					pieceController.SetDimension (i, j, k);

					// GameObjectの参照を設定
					pieceObjArray [i, j, k] = pieceObj;

					// GameObjectの座標を設定
					pieceObj.transform.position = pieceArray [i, j, k].position;
				}
			}
		}

		// 1段目以降を非表示
		// GetLengthは配列の長さ
		// GetLength(0)は、1次元目(つまりX)の配列の箱の数。この場合は4。
		// GetLength(1)は、2次元目(つまりY)の配列の箱の数。この場合は4。
		// GetLength(2)は、3次元目(つまりZ)の配列の箱の数。この場合は4。

		for (int i = 0; i < pieceArray.GetLength (0); i++) {
			for (int j = 0; j < pieceArray.GetLength (1); j++) {
				for (int k = 0; k < pieceArray.GetLength (2); k++) {

					// Y軸が2段目以降の場合、オブジェクトを非アクティブにする。
					if (j > 0) {
						pieceObjArray [i, j, k].gameObject.SetActive (false);
					}
				}
			}
		}

	}
	
	void Update () {

		// 左クリックした際の処理
		if (Input.GetMouseButtonDown (0)) {

			// クリックしたスクリーン座標をRayに変換
			// Input.mousePositionにはVector2が入っている
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			// Rayの当たったオブジェクトの情報を格納する
			// 初期化処理
			RaycastHit hit = new RaycastHit ();

			// オブジェクトにrayが当たった時
			// Physics.Raycast関数の引数
			// 第一引数: 当たり判定で使用するRayを飛ばす
			// 第二引数: hitを渡して値を更新してhitを戻すようなイメージ
			// 第三引数: Rayを飛ばす距離(長さ)を指定。Mathf.Infinityで無限。
			// 第四引数: LayerMask。LayerMask型の変数。レイヤーを指定してそのレイヤーにだけRayが当たるようにする。
			if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
				PieceController pieceController = hit.collider.gameObject.GetComponent<PieceController> ();

				// Rayが当たったコマの情報を取得する
				int x = pieceController.GetDimensionX ();
				int y = pieceController.GetDimensionY ();
				int z = pieceController.GetDimensionZ ();

				Piece piece = pieceArray [x, y, z];

				// コマの情報を取得
				if (piece.pieceType == PieceType.None) {

					// 選択されているコマがない場合
					if (selectedPieceController == null) {

						// クリックした時にコマを表示する
						pieceController.ShowPiece ();

						// 最初にクリックしたら赤色になる
						pieceController.ChangeMaterial (PieceType.Red);

						// Rayが当たったオブジェクト(pieceController)をプライベート変数に入れておく
						selectedPieceController = pieceController;
					
					
					// 選択されているコマがある場合
					} else {

						// 同じコマを選択した場合
						if (pieceController == selectedPieceController) {

							piece.SetPieceType (order);

							pieceController.ChangeMaterial (order);

							GameCheck ();

							selectedPieceController = null;

							// ターンを変更する
							if (order == PieceType.Black) {
								order = PieceType.White;
							} else if (order == PieceType.White) {
								order = PieceType.Black;
							}

							// アクティブにしたコマが、4段目ではない場合に
							// アクティブにしたコマの1段上をアクティブにする
							// オブジェクトは見えないが、コライダーがあるのでRayが当たるようになる
							if (y < pieceYCount - 1) {
								GameObject pieceObj = pieceObjArray [x, y + 1, z];
								pieceObj.SetActive (true);
							}
						}

						// 違うコマを選択した場合
						else {

							// 現在の赤いコマを非表示にする
							selectedPieceController.HidePiece();

							// 新しく選択したコマを赤く変更する
							pieceController.ShowPiece();
							pieceController.ChangeMaterial (PieceType.Red);

							// Rayが当たったオブジェクト(pieceController)をプライベート変数に入れておく
							selectedPieceController = pieceController;
						}
					}
				}
			}
		}
	}

	void GameCheck () {

		PieceType checkPieceType = PieceType.Initial;

		int checkPieceCount = 0;

		// 縦のチェック
		for (int x = 0; x < 4; x++) {

			for (int y = 0; y < 4; y++) {

				for (int z = 0; z < 4; z++) {

					if (z == 0) {

						checkPieceType = pieceArray [x, y, z].pieceType;

						checkPieceCount = 0;

						if (checkPieceType == PieceType.None) {


							break;

						}

					} else {

						if (checkPieceType == pieceArray [x, y, z].pieceType) {

							checkPieceCount++;

							if (checkPieceType == pieceArray [x, y, z].pieceType && checkPieceCount == 3) {

								Debug.Log ("WIN");

							}
						} else {

							break;

						}
					}
				}
			}
		}

		// 横のチェック
		for (int z = 0; z < 4; z++) {

			for (int y = 0; y < 4; y++) {

				for (int x = 0; x < 4; x++) {

					if (x == 0) {

						checkPieceType = pieceArray [x, y, z].pieceType;

						checkPieceCount = 0;

						if (checkPieceType == PieceType.None) {


							break;

						}

					} else {

						if (checkPieceType == pieceArray [x, y, z].pieceType) {

							checkPieceCount++;

							if (checkPieceType == pieceArray [x, y, z].pieceType && checkPieceCount == 3) {

								Debug.Log ("WIN");

							}
						} else {

							break;

						}
					}
				}
			}
		}

		// 高さのチェック
		for (int x = 0; x < 4; x++) {

			for (int z = 0; z < 4; z++) {

				for (int y = 0; y < 4; y++) {

					if (y == 0) {

						checkPieceType = pieceArray [x, y, z].pieceType;

						checkPieceCount = 0;

						if (checkPieceType == PieceType.None) {

							break;

						}

					} else {

						if (checkPieceType == pieceArray [x, y, z].pieceType) {

							checkPieceCount++;

							if (checkPieceType == pieceArray [x, y, z].pieceType && checkPieceCount == 3) {

								Debug.Log ("WIN");

							}
						} else {

							break;

						}
					}
				}
			}
		}

		// 左下から右上に斜め。Z(奥行をずらしていく)
		for (int z = 0; z < 4; z++) {

			checkPieceType = pieceArray [0, 0, z].pieceType;

			if (checkPieceType == PieceType.None) {

				continue;

			}

			if (checkPieceType == pieceArray [1, 1, z].pieceType &&
				checkPieceType == pieceArray [2, 2, z].pieceType && 
				checkPieceType == pieceArray [3, 3, z].pieceType ) {

				Debug.Log ("WIN");
			}
		}

		// 右下から左上に斜め。Z(奥行をずらしていく)
		for (int z = 0; z < 4; z++) {

			checkPieceType = pieceArray [3, 0, z].pieceType;

			if (checkPieceType == PieceType.None) {

				continue;

			}
			if (checkPieceType == pieceArray [2, 1, z].pieceType &&
				checkPieceType == pieceArray [1, 2, z].pieceType && 
				checkPieceType == pieceArray [0, 3, z].pieceType ) {

				Debug.Log ("WIN");
			}
		}

		// 左下手前から左奥に斜め。X(横方向をずらしていく)
		for (int x = 0; x < 4; x++) {

			checkPieceType = pieceArray [x, 0, 0].pieceType;

			if (checkPieceType == PieceType.None) {

				continue;

			}

			if (checkPieceType == pieceArray [x, 1, 1].pieceType &&
				checkPieceType == pieceArray [x, 2, 2].pieceType && 
				checkPieceType == pieceArray [x, 3, 3].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 左下奥から左手前に斜め。X(横方向をずらしていく)
		for (int x = 0; x < 4; x++) {

			checkPieceType = pieceArray [x, 0, 3].pieceType;

			if (checkPieceType == PieceType.None) {

				continue;

			}

			if (checkPieceType == pieceArray [x, 1, 2].pieceType &&
				checkPieceType == pieceArray [x, 2, 1].pieceType && 
				checkPieceType == pieceArray [x, 3, 0].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 左下からクロス。Y(高さをずらしていく)
		for (int y = 0; y < 4; y++) {

			checkPieceType = pieceArray [0, y, 0].pieceType;

			if (checkPieceType == PieceType.None) {

				continue;

			}

			if (checkPieceType == pieceArray [1, y, 1].pieceType &&
				checkPieceType == pieceArray [2, y, 2].pieceType && 
				checkPieceType == pieceArray [3, y, 3].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 右下からクロス。Y(高さをずらしていく)
		for (int y = 0; y < 4; y++) {

			checkPieceType = pieceArray [3, y, 0].pieceType;

			if (checkPieceType == PieceType.None) {

				continue;

			}

			if (checkPieceType == pieceArray [2, y, 1].pieceType &&
				checkPieceType == pieceArray [1, y, 2].pieceType && 
				checkPieceType == pieceArray [0, y, 3].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 左手前下から右奥上に斜めクロス
		checkPieceType = pieceArray [0, 0, 0].pieceType;

		if (checkPieceType != PieceType.None) {

			if (checkPieceType == pieceArray [1, 1, 1].pieceType &&
				checkPieceType == pieceArray [2, 2, 2].pieceType && 
				checkPieceType == pieceArray [3, 3, 3].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 右手前下から左奥上に斜めクロス
		checkPieceType = pieceArray [3, 0, 0].pieceType;

		if (checkPieceType != PieceType.None) {

			if (checkPieceType == pieceArray [2, 1, 1].pieceType &&
				checkPieceType == pieceArray [1, 2, 2].pieceType && 
				checkPieceType == pieceArray [0, 3, 3].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 左奥下から右手前上に斜めクロス
		checkPieceType = pieceArray [0, 0, 3].pieceType;

		if (checkPieceType != PieceType.None) {

			if (checkPieceType == pieceArray [1, 1, 2].pieceType &&
				checkPieceType == pieceArray [2, 2, 1].pieceType && 
				checkPieceType == pieceArray [3, 3, 0].pieceType ) {

				Debug.Log ("WIN");

			}
		}

		// 右奥下から左手前上に斜めクロス
		checkPieceType = pieceArray [3, 0, 3].pieceType;

		if (checkPieceType != PieceType.None) {

			if (checkPieceType == pieceArray [2, 1, 2].pieceType &&
				checkPieceType == pieceArray [1, 2, 1].pieceType && 
				checkPieceType == pieceArray [0, 3, 0].pieceType ) {

				Debug.Log ("WIN");

			}
		}
	}

	void CpuLogic(){

		// 現在の状況をコピーする
		Piece[, ,] pieceArrayNow = new Piece[pieceXCount, pieceYCount, pieceZCount];

		Array.Copy(pieceArray, pieceArrayNow, pieceArray.Length);

		// 勝てばポイントが増える三次元配列
		int[, ,] piecePoints = new int[pieceXCount, pieceYCount, pieceZCount];

		// CPUが、置いたら勝てる場所を探してあれば置く

		// CPUが、置かないと負ける場所を探してあれば置く

		// CPUが、16パターン試して、勝敗引き分けを格納


		for (int z = 0; z < 4; z++) {

			for (int x = 0; x < 4; x++) {

				for (int y = 0; y < 4; y++) {
					
					if (pieceArrayNow [x, y, z].pieceType == PieceType.None) {

						// この座標にコマを置いてテスト
						// ここに関数
						CpuGameCheck();

						Debug.Log (pieceArrayNow [x, y, z].position);

						break;

					} else {

						// 1段上のコマをチェック
						continue;

					}
				}
			}
		}
		// ゲーム難易度に応じて、どこに置くかを決める

	}

	void CpuGameCheck(PieceType cpuPieceType){

		pieceArrayNow [x, y, z].pieceType = cpuPieceType;

	}
}
