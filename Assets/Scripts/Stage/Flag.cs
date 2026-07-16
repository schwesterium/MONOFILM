using UnityEngine;

//=====担当外のスクリプト=====

public class Flag : MonoBehaviour
{
    [SerializeField]
    private StageScene stageScene;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            stageScene.StageClear();  // ゲームオーバー処理の呼び出し
        }
    }
}
