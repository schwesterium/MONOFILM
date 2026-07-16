using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageScene : MonoBehaviour
{
    //ロードするシーンを指定する
    [SerializeField]
    [Tooltip("ロードするシーンを指定する")]
    private string loadSceneName = "";

    [SerializeField]
    private Player player = null;
    [SerializeField]
    private SwtichGround switchGround = null;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera = null;

    [Header("UI関連")]
    [SerializeField]
    GameOverUI gameOverUI = null;
    [SerializeField]
    StageClearUI stageClearUI = null;
    [SerializeField]
    PauseUI pauseUI = null;

    [Header("音声")]
    [SerializeField]
    private AudioClip pauseClip = null;

    //ポーズ中か否かを表す変数
    private bool isPause = false;

    //ステージの進行状態
    enum StageState
    {
        //開始演出中
        Intro,
        //プレイ中
        Play,
        //ステージクリア
        StageClear,
        //ゲームオーバー
        GameOver
    }

    StageState currentState = StageState.Play;

    [SerializeField]
    AudioSource bgmSource;
    [SerializeField]
    AudioSource seSource;

    private Animator animator;

    static readonly int outroId = Animator.StringToHash("Outro");
    //アニメーションの時間
    private readonly float animTime = 1.3f;
    //ポーズできるかどうか
    private bool canPause = false;

#if UNITY_EDITOR
    //EditorでのみSettingManagerを生成する
    //シーン上に一個一個置くのは面倒だね
    [Header("SettingManager")]
    [SerializeField]
    private GameObject settingManager;

    private void CreateSettingManager()
    {
        Instantiate(settingManager);
    }
#endif

    private void Awake()
    {
#if UNITY_EDITOR
        CreateSettingManager();
#endif

        animator = GetComponent<Animator>();
        canPause = false;

        //コールバックの登録
        gameOverUI.OnRestartButtonClick.AddListener(Restart);
        gameOverUI.OnBackTitleButtonClick.AddListener(BackTitle);
        stageClearUI.OnNextButtonClick.AddListener(LoadAssignedScene);
        stageClearUI.OnBackTitleButtonClick.AddListener(BackTitle);
        stageClearUI.OnQuitButtonClick.AddListener(Quit);
        pauseUI.onResumeButtonClick.AddListener(Pause);
        pauseUI.onTitleButtonClick.AddListener(BackTitle);
    }

    private void Start()
    {
        AllowPause();
    }

    private void AllowPause()
    {
        StartCoroutine(OnAllowPause());
    }

    //イントロアニメーションの再生時間だけ待ってからポーズができるようにする
    private IEnumerator OnAllowPause()
    {
        yield return new WaitForSeconds(animTime);
        canPause = true;

    }

    //指定したシーンをロードする
    public void LoadAssignedScene()
    {
        StartCoroutine(OnLoadAssignedScene(loadSceneName));
    }

    private IEnumerator OnLoadAssignedScene(string sceneName)
    {
        canPause = false;
        animator.SetTrigger(outroId);
        //アニメーションの再生終了まで待つ
        yield return new WaitForSeconds(animTime);
        //yield return null;

        SceneManager.LoadScene(sceneName);
    }

    //ゲームオーバー
    public void GameOver()
    {
        if (currentState == StageState.Play)
        {
            currentState = StageState.GameOver;
            bgmSource.Stop();
            player.Sleep();
            gameOverUI.AnimPlay();
            virtualCamera.gameObject.SetActive(false);
        }

    }

    //ステージクリア
    public void StageClear()
    {
        if (currentState == StageState.Play)
        {
            currentState = StageState.StageClear;
            bgmSource.Stop();
            player.Sleep();
            stageClearUI.AnimPlay();
            virtualCamera.gameObject.SetActive(false);
        }

    }

    //InputSystemから呼ぶメソッド
    public void OnPause(InputAction.CallbackContext context)
    {
        if (currentState == StageState.Play && context.started)
        {
            seSource.PlayOneShot(pauseClip);
            Pause();
        }
    }

    //白黒の切り替え 地面、プレイヤー両方
    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (currentState == StageState.Play && !isPause && context.started)
        {
            player.ColourChange();
            switchGround.Switch();
        }
    }

    //ポーズの切り替え
    private void Pause()
    {
        if (canPause)
        {
            if (!isPause)//ポーズ
            {
                isPause = true;
                player.Sleep();
                Time.timeScale = 0f;
                pauseUI.Show();
                virtualCamera.gameObject.SetActive(false);

                return;
            }

            if (isPause)//ポーズ解除
            {
                isPause = false;
                player.WakeUp();
                Time.timeScale = 1f;
                pauseUI.Hide();
                virtualCamera.gameObject.SetActive(true);

                return;

            }
        }
    }

    //リスタート
    public void Restart()
    {
        if (isPause) { Pause(); }
        StartCoroutine(OnLoadAssignedScene(SceneManager.GetActiveScene().name));
        virtualCamera.gameObject.SetActive(true);
    }

    //タイトルへ戻る
    public void BackTitle()
    {
        if (isPause) { Pause(); }
        StartCoroutine(OnLoadAssignedScene("TitleScene"));

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
