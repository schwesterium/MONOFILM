using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleScene : MonoBehaviour
{
    //ロードするシーンを記述する
    [SerializeField]
    private string loadSceneName = "";

    static readonly int outroId = Animator.StringToHash("Outro");
    private Animator animator;

    [SerializeField]
    private InputAction inputAction;

    [SerializeField]
    private OptionUI optionUI = null;

    //ロード可能かどうか
    private bool isLoadable = false;

    //InputAction有効化
    private void OnEnable()
    {
        //コールバックの登録
        inputAction.performed += LoadExStage;
        //これをしないと入力を受け取れない
        inputAction?.Enable();
    }

    //InputAction無効化
    private void OnDisable()
    {
        inputAction.performed -= LoadExStage;
        inputAction?.Disable();
    }

    private void LoadExStage(InputAction.CallbackContext context)
    {
        //exステージをロードする
        if (isLoadable && optionUI.Hidding) {StartCoroutine(OnLoadAssignedScene("StageH05")); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        //マウスカーソルを表示、カーソルはウィンドウ内に制限
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(OnStart());
    }

    //指定したシーンをロードする
    public void LoadAssignedScene()
    {
        if (isLoadable && optionUI.Hidding) { StartCoroutine(OnLoadAssignedScene(loadSceneName)); }
    }

    private IEnumerator OnLoadAssignedScene(string sceneName)
    {
        animator.SetTrigger(outroId);
        //アウトロのアニメーションの再生が終わるまで待つ
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator OnStart()
    {
        //この引数はIntroアニメーションのボタンが有効化タイミング
        yield return new WaitForSeconds(1.5f);
        isLoadable = true;
    }

    //アプリの終了
    //Editorの場合はプレイモードを終了する
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
