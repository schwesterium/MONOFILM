using Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class OptionUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("このUIを閉じるボタンを指定する")]
    private Button backButton = null;

    [SerializeField]
    [Tooltip("背後にあるボタン郡を指定する")]
    private GameObject Buttons = null;

    [SerializeField]
    [Tooltip("このUIを閉じたときに選択するボタンを指定する")]
    private Button SelectButton = null;

    [Header("Setting")]

    private CinemachinePOV cinePov;

    [SerializeField]
    [Tooltip("BGMスライダーを指定する")]
    private Slider bgmSlider = null;
    [SerializeField]
    [Tooltip("SEスライダーを指定する")]
    private Slider seSlider = null;
    [SerializeField]
    [Tooltip("Sensiスライダーを指定する")]
    private Slider sensiSlider = null;

    private AudioMixer audioMixer;
    //BGM,SEのデシベルを格納する変数
    private float dbBGM;
    private float dbSE;
    private float sensi;

    private bool hidding = false;
    public bool Hidding { get { return hidding; } }

    //PauseUIがhideできるかどうか
    private bool canHide = false;
    public bool CanHide { get { return canHide; } }

    private void Awake()
    {
        hidding = false;

        //カメラ感度の調整に必要なコンポーネントの取得
        cinePov = FindAnyObjectByType<CinemachineVirtualCamera>()?.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Start()
    {
        Initialize();

        canHide = true;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        hidding = true;

        //BGM調整
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            //ボリュームをデシベルに変換する(log10をとって20をかける)
            float decibel = 20f * Mathf.Log10(value);
            //-80から0にClampする
            decibel = Mathf.Clamp(decibel, -80f, 0f);

            audioMixer.SetFloat("BGMParam", decibel);
            SettingManager.instance.decibelBGM = decibel;
        });
        //SE調整
        seSlider.onValueChanged.AddListener((value) =>
        {
            //ボリュームをデシベルに変換する(log10をとって20をかける)
            float decibel = 20f * Mathf.Log10(value);
            //-80から0にClampする
            decibel = Mathf.Clamp(decibel, -80f, 0f);

            audioMixer.SetFloat("SEParam", decibel);
            SettingManager.instance.decibelSE = decibel;
        });
        //カメラ感度調整
        sensiSlider.onValueChanged.AddListener((value) =>
        {
            //コントローラーの感度が最大でも遅いためmaxspeedの変更は2倍する
            var v = value * 2;

            if (cinePov != null)
            {
                cinePov.m_VerticalAxis.m_MaxSpeed = v;
                cinePov.m_HorizontalAxis.m_MaxSpeed = v;
            }

            SettingManager.instance.camSensi = v;
        });
    }

    /// <summary>
    /// Hide,ShowはPauseUIから行う
    /// </summary>

    //設定画面を表示する
    //その際背後にあるボタンを非表示にする
    public void Show()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        hidding = false;

        Buttons.SetActive(false);

        backButton.Select();
    }

    //設定画面を非表示する
    //その際背後にあるボタンを表示にする
    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        hidding = true;

        Buttons.SetActive(true);

        SelectButton.Select();
    }

    public void Initialize()
    {
        audioMixer = SettingManager.instance.AudioMixer;
        dbBGM = SettingManager.instance.decibelBGM;
        dbSE = SettingManager.instance.decibelSE;
        sensi = SettingManager.instance.camSensi;

        //スライダーの値を指定する
        bgmSlider.value = Mathf.Pow(10f, dbBGM / 20f);
        seSlider.value = Mathf.Pow(10f, dbSE / 20f);
        //sensiは2倍された状態で値が渡されるので、スライダーに適応するときは1/2する
        sensiSlider.value = sensi * 0.5f;

        //音量の設定
        audioMixer.SetFloat("BGMParam", dbBGM);
        audioMixer.SetFloat("SEParam", dbSE);

        //カメラ感度の調整
        if (cinePov != null)
        {
            cinePov.m_VerticalAxis.m_MaxSpeed = sensi;
            cinePov.m_HorizontalAxis.m_MaxSpeed = sensi;
        }
    }
}
