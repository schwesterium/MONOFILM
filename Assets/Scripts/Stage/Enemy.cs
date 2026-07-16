using UnityEngine;

//=====担当外のスクリプト=====

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private StageScene stageScene = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            stageScene.GameOver();  // ゲームオーバー処理の呼び出し
        }
    }
}