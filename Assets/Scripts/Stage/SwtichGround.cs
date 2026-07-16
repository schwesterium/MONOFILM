using UnityEngine;

public class SwtichGround : MonoBehaviour
{
    [Header("ステージ")]
    //白と黒の足場を指定する
    [SerializeField]
    private Transform whiteRoot = null;
    [SerializeField]
    private Transform blackRoot = null;

    //白・黒の足場のコライダーをそれぞれ格納する
    private Collider[] whiteCol = { };
    private Collider[] blackCol = { };

    //今の足場が白であるかどうかを判別するフラグ
    private bool isWhite = true;

    private void Awake()
    {
        whiteCol = whiteRoot.GetComponentsInChildren<Collider>();
        blackCol = blackRoot.GetComponentsInChildren<Collider>();

        BlackGroundColliderEnable(false);
    }

    //白黒の切り替え
    public void Switch()
    {
        //トグル
        isWhite = !isWhite;

        //白を有効にしている場合は白の足場のコライダーを有効化し黒は無効化、黒が有効の場合はこれの逆を行う
        if (isWhite)
        {
            WhiteGroundColliderEnable(true);
            BlackGroundColliderEnable(false);
        }
        else
        {
            WhiteGroundColliderEnable(false);
            BlackGroundColliderEnable(true);
        }

    }

    private void WhiteGroundColliderEnable(bool b)
    {
        foreach (BoxCollider col in whiteCol)
        {
            col.enabled = b;
        }
    }

    private void BlackGroundColliderEnable(bool b)
    {
        foreach (BoxCollider col in blackCol)
        {
            col.enabled = b;
        }
    }

}
