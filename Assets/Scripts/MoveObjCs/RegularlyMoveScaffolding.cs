using UnityEngine;

public class RegularlyMoveScaffolding : MonoBehaviour
{
    [SerializeField]
    private float speed = 0f;

    [SerializeField]
    [Tooltip("足場")]
    private Transform scaffolding = null;
    [SerializeField]
    [Tooltip("最初のオブジェクトが始点、最後のオブジェクトが終点になる")]
    private Transform movePoints = null;
    [SerializeField]
    [Tooltip("到着とみなす距離の閾値を決める")]
    private float arriveThreshold = 0.2f;

    //移動ポイントの配列
    private Transform[] points;
    private int currentIndex = 0;

    //次に向かうポイント
    private Vector3 targetPoint = Vector3.zero;

    private Rigidbody rb;

    private void Awake()
    {
        rb = scaffolding.GetComponent<Rigidbody>();
        points = movePoints.GetComponentsInChildren<Transform>();

        //配列0番目は親オブジェクトが格納されるため除外する
        currentIndex = 1;

        //初期位置を指定する
        rb.position = points[currentIndex].position;
        //次に向かうポイントを指定する
        targetPoint = points[currentIndex + 1].position;
    }

    //points配列に登録された位置を順に通過し、最後のポイントに到達したら最初のポイントに移動する
    void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPoint, speed * Time.fixedDeltaTime));

        //ポイントに到着したら
        if ((rb.position - targetPoint).magnitude < arriveThreshold)
        {
            //次のポイントのインデクスにする
            ++currentIndex;

            //配列の要素数以上の場合は最初のポイントに戻す
            if (currentIndex >= points.Length)
            {
                currentIndex = 1;
            }

            //次の移動先を設定
            targetPoint = points[currentIndex].position;
        }
    }
}
