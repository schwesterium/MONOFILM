using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearScene : MonoBehaviour
{
    //ロードするシーンを記述する
    [SerializeField]
    private string loadSceneName = "";

    static readonly int outroId = Animator.StringToHash("Outro");
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //指定したシーンをロードする
    public void LoadAssignedScene()
    {
        StartCoroutine(OnLoadAssignedScene(loadSceneName));
    }

    private IEnumerator OnLoadAssignedScene(string sceneName)
    {
        animator.SetTrigger(outroId);
        //アニメーションの再生時間分待つ
        yield return new WaitForSeconds(2.2f);

        SceneManager.LoadScene(sceneName);
    }
}
