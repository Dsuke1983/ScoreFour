using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType {
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

	// 生成するコマのPrefab
	public GameObject piecePrefab;

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

	private Piece[, ,] pieceArray = new Piece[pieceXCount, pieceYCount, pieceZCount];

	private GameObject[, ,] pieceObjArray = new GameObject[pieceXCount, pieceYCount, pieceZCount];

	void Start () {

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
		if (Input.GetMouseButton (0)) {

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

				// クリックした時にコマを表示する
				pieceController.ShowPiece ();

				// コマのデータを更新する
				int x = pieceController.GetDimensionX ();
				int y = pieceController.GetDimensionY ();
				int z = pieceController.GetDimensionZ ();
				Piece piece = pieceArray [x, y, z];

				// コマの情報を取得
				if (piece.pieceType == PieceType.None) {
					piece.SetPieceType (PieceType.White);

					// アクティブにしたコマが、4段目ではない場合に
					// アクティブにしたコマの1段上をアクティブにする
					// オブジェクトは見えないが、コライダーがあるのでRayが当たるようになる
					if (y < pieceYCount - 1) {
						GameObject pieceObj = pieceObjArray [x, y + 1, z];
						pieceObj.SetActive (true);
					}
				}
			}
		}
	}
}
