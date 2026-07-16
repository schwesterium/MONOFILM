using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [Header("プレイヤーステータス")]
    //移動速度
    [SerializeField]
    private float moveSpeed = 5f;
    // ジャンプ力
    [SerializeField]
    private Vector3 jumpForce = new(0, 5f, 0);

    [SerializeField]
    [Tooltip("追加でジャンプできる回数を指定する")]
    private int addJump = 1;
    //追加でジャンプした回数を記録する変数
    private int jumpCount = 0;
    //自分が白であるかどうか
    private bool isWhite = true;

    //物理演算の影響がどうなっているかを判断する
    private bool isSleep;

    //MoveActionの入力値
    private Vector2 moveInput = Vector2.zero;
    private Vector3 moveDirection = Vector3.zero;

    //お化けのモデル
    [SerializeField]
    private GameObject ghostWh = null;
    [SerializeField]
    private GameObject ghostBl = null;

    [Header("Ray関連")]
    [SerializeField]
    private Vector3 groundRayOffset = Vector3.zero;//rayを発射するための原点となる位置
    [SerializeField]
    private float groundRayLength = 1.0f;//rayの長さ
    private Ray groundRay;
    private bool isGround;

    [SerializeField]
    private Vector3 wallRayOffset = Vector3.zero;
    [SerializeField]
    private float wallRayLength = 1.0f;
    [SerializeField]
    [Tooltip("壁と判定するレイヤーを指定します")]
    private LayerMask wallLayer;
    private Ray wallRay;
    private bool isWall;

    [Header("エフェクト")]
    [SerializeField]
    [Tooltip("色切り替え時のエフェクト")]
    private GameObject effectWhChg = null;
    [SerializeField]
    [Tooltip("色切り替え時のエフェクト")]
    private GameObject effectBlChg = null;

    [SerializeField]
    [Tooltip("ジャンプ時のエフェクト")]
    private GameObject effectWhJump = null;
    [SerializeField]
    [Tooltip("ジャンプ時のエフェクト")]
    private GameObject effectBlJump = null;

    [SerializeField]
    private Vector3 jumpPosOffset = Vector3.zero;

    [Header("音声")]
    [SerializeField]
    AudioClip jumpClip = null;
    [SerializeField]
    AudioClip changeBtWClip = null;
    [SerializeField]
    AudioClip changeWtBClip = null;

    //動く足場のRigidbodyを格納する変数
    private Rigidbody scaffoldingRb = null;
    //動く足場の移動速度を格納する変数
    private Vector3 scaffoldingVel = Vector3.zero;

    //コンポーネントを事前に参照しておく変数
    private Rigidbody rb;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        isWhite = true;

        ghostWh.SetActive(true);
        ghostBl.SetActive(false);
    }

    void Start()
    {
        isSleep = false;
    }

    void Update()
    {
        PlayerRotate();

        Move();
    }

    private void FixedUpdate()
    {
        JumpDecision();
        Wall();
    }

    //カメラのY軸回転をプレイヤーにも回転させる
    private void PlayerRotate()
    {
        var playerRot = transform.rotation.eulerAngles;
        playerRot.y = Camera.main.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(playerRot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //動く足場のrigidbodyを取得する
        if (collision.collider.CompareTag("MoveScaffolding"))
        {
            scaffoldingRb = collision.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //参照していたrigidbodyをnullにする
        if (collision.collider.CompareTag("MoveScaffolding"))
        {
            scaffoldingRb = null;
            scaffoldingVel = Vector3.zero;
        }
    }

    //地面にいるかどうかを判定して、trueならジャンプ可能回数をリセットする
    private void JumpDecision()
    {
        groundRay = new Ray(transform.position + groundRayOffset, Vector3.down);
        isGround = Physics.Raycast(groundRay, groundRayLength);

        if (isGround)
        {
            jumpCount = 0;
        }
    }

    private void Move()
    {
        if (isSleep) return;

        //メインカメラの前方と右方向を取得
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        //カメラのy軸方向を無視して、地面に沿った移動にする
        cameraForward.y = 0;
        cameraRight.y = 0;

        //正規化して、カメラの前方と右方向に基づいた移動ベクトルを計算
        moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        var vel = rb.linearVelocity;
        //動く足場に乗っているときのみ足場の速度を取得する
        if (scaffoldingRb != null) scaffoldingVel = scaffoldingRb.linearVelocity;

        if (isWall)
        {
            //壁に当たっている場合は足場の速度をセット
            vel.x = scaffoldingVel.x;
            vel.z = scaffoldingVel.z;
        }
        else
        {
            //通常はプレイヤーの速度速度に加えて足場の移動速度を加える
            vel.x = (moveDirection.x * moveSpeed) + scaffoldingVel.x;
            vel.z = (moveDirection.z * moveSpeed) + scaffoldingVel.z;
        }

        rb.linearVelocity = vel;
    }

    //壁に当たったかどうかを判定する
    private void Wall()
    {
        wallRay = new Ray(transform.position + wallRayOffset, moveDirection);
        isWall = Physics.Raycast(wallRay, wallRayLength, wallLayer);
    }

    void OnDrawGizmos()
    {
        //rayの描画
        Gizmos.color = Color.red;
        Gizmos.DrawRay(wallRay);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + groundRayOffset, Vector3.down);
    }

    private void Jump()
    {
        //ジャンプできる回数を超えるまではジャンプが可能
        if (jumpCount < addJump)
        {
            //白黒に応じてエフェクトを切り替えて生成
            if (isWhite)
            {
                var obj = Instantiate(effectWhJump, transform.position + jumpPosOffset, Quaternion.identity, transform.root);
                Destroy(obj, 1);
            }
            else
            {
                var obj = Instantiate(effectBlJump, transform.position + jumpPosOffset, Quaternion.identity, transform.root);
                Destroy(obj, 1);
            }

            //力を加える
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(jumpForce, ForceMode.Impulse);

            audioSource.PlayOneShot(jumpClip);
            if (!isGround)
            {
                jumpCount++;
            }// 2回目以降のジャンプのみカウントする
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isSleep && context.started) Jump();
    }

    //物理演算の影響を受けなくさせる
    public void Sleep()
    {
        isSleep = true;
        rb.isKinematic = true;
    }

    //物理演算の影響を受けさせる
    public void WakeUp()
    {
        isSleep = false;
        rb.isKinematic = false;
    }


    //お化け白とお化け黒を切り替えを行う
    public void ColourChange()
    {
        if (isSleep) { return; }

        //切り替え
        if (isWhite)
        {
            var obj = Instantiate(effectBlChg, transform.position, Quaternion.identity, transform.root);
            Destroy(obj, 1);
            audioSource.PlayOneShot(changeBtWClip);
            ghostWh.SetActive(false);
            ghostBl.SetActive(true);
            isWhite = false;
        }
        else
        {
            var obj = Instantiate(effectWhChg, transform.position, Quaternion.identity, transform.root);
            Destroy(obj, 1);
            audioSource.PlayOneShot(changeWtBClip);
            ghostWh.SetActive(true);
            ghostBl.SetActive(false);
            isWhite = true;
        }

    }
}